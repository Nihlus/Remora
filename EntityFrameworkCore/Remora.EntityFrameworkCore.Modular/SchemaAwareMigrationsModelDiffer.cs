//
//  SchemaAwareMigrationsModelDiffer.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;

#pragma warning disable EF1001 // Internal EF API usage

namespace Remora.EntityFrameworkCore.Modular
{
    /// <summary>
    /// Generates differences between migrations, taking the schema of the model entities into account.
    /// </summary>
    internal class SchemaAwareMigrationsModelDiffer : MigrationsModelDiffer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaAwareMigrationsModelDiffer"/> class.
        /// </summary>
        /// <param name="typeMappingSource">The type mapping source to use.</param>
        /// <param name="migrationsAnnotations">The migration annotation provider.</param>
        /// <param name="changeDetector">The change detector.</param>
        /// <param name="updateAdapterFactory">The update adapter factory.</param>
        /// <param name="commandBatchPreparerDependencieses">The command batch preparer dependencies.</param>
        public SchemaAwareMigrationsModelDiffer
        (
            [NotNull] IRelationalTypeMappingSource typeMappingSource,
            [NotNull] IMigrationsAnnotationProvider migrationsAnnotations,
            [NotNull] IChangeDetector changeDetector,
            [NotNull] IUpdateAdapterFactory updateAdapterFactory,
            [NotNull] CommandBatchPreparerDependencies commandBatchPreparerDependencieses
        )
            : base
            (
                typeMappingSource,
                migrationsAnnotations,
                changeDetector,
                updateAdapterFactory,
                commandBatchPreparerDependencieses
            )
        {
        }

        /// <inheritdoc/>
        public override bool HasDifferences(IModel source, IModel target)
            => Diff(source, target, new SchemaAwareDiffContext(source, target)).Any();

        /// <inheritdoc/>
        public override IReadOnlyList<MigrationOperation> GetDifferences
        (
            IModel source,
            IModel target
        )
        {
            var diffContext = new SchemaAwareDiffContext(source, target);
            return Sort(Diff(source, target, diffContext), diffContext);
        }

        private static ReferentialAction ToReferentialAction(DeleteBehavior deleteBehavior)
            => deleteBehavior == DeleteBehavior.Cascade
                ? ReferentialAction.Cascade
                : deleteBehavior == DeleteBehavior.SetNull
                    ? ReferentialAction.SetNull
                    : ReferentialAction.Restrict;

        /// <inheritdoc/>
        protected override IEnumerable<MigrationOperation> Diff(
            IEnumerable<IForeignKey> source,
            IEnumerable<IForeignKey> target,
            DiffContext diffContext)
        {
            return DiffCollection
            (
                source,
                target,
                diffContext,
                Diff,
                Add,
                Remove,
                (s, t, c) =>
                {
                    if (s.GetConstraintName() != t.GetConstraintName())
                    {
                        return false;
                    }

                    if (!s.Properties.Select(p => p.GetColumnName()).SequenceEqual(
                        t.Properties.Select(p => c.FindSource(p)?.GetColumnName())))
                    {
                        return false;
                    }

                    var schemaToInclude = ((SchemaAwareDiffContext)diffContext).Source?.GetDefaultSchema();
                    if (schemaToInclude is null)
                    {
                        return false;
                    }

                    if (c.FindSourceTable(s.PrincipalEntityType).Schema == schemaToInclude &&
                        c.FindSourceTable(s.PrincipalEntityType) !=
                        c.FindSource(c.FindTargetTable(t.PrincipalEntityType)))
                    {
                        return false;
                    }

                    if (t.PrincipalKey.Properties.Select(p => c.FindSource(p)?.GetColumnName())
                            .First() != null && !s.PrincipalKey.Properties
                            .Select(p => p.GetColumnName()).SequenceEqual(
                                t.PrincipalKey.Properties.Select(p =>
                                    c.FindSource(p)?.GetColumnName())))
                    {
                        return false;
                    }

                    if (ToReferentialAction(s.DeleteBehavior) != ToReferentialAction(t.DeleteBehavior))
                    {
                        return false;
                    }

                    return !HasDifferences
                    (
                        this.MigrationsAnnotations.For(s),
                        this.MigrationsAnnotations.For(t)
                    );
                }
            );
        }

        /// <summary>
        /// Provides a schema aware diffing context for the migration differ.
        /// </summary>
        protected class SchemaAwareDiffContext : DiffContext
        {
            /// <summary>
            /// Gets the source model.
            /// </summary>
            [CanBeNull]
            public IModel Source { get; }

            /// <summary>
            /// Gets the target model.
            /// </summary>
            [CanBeNull]
            public IModel Target { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="SchemaAwareDiffContext"/> class.
            /// </summary>
            /// <param name="source">The source model.</param>
            /// <param name="target">The target model.</param>
            public SchemaAwareDiffContext([CanBeNull] IModel source, [CanBeNull] IModel target)
                : base(source, target)
            {
                this.Source = source;
                this.Target = target;
            }

            /// <inheritdoc/>
            [NotNull, ItemNotNull]
            public override IEnumerable<TableMapping> GetSourceTables()
            {
                if (this.Source is null)
                {
                    return new TableMapping[] { };
                }

                var schemaToInclude = this.Source.GetDefaultSchema();
                var tables = base.GetSourceTables();

                return tables.Where(x => x.Schema == schemaToInclude);
            }

            /// <inheritdoc/>
            [NotNull, ItemNotNull]
            public override IEnumerable<TableMapping> GetTargetTables()
            {
                if (this.Target is null)
                {
                    return new TableMapping[] { };
                }

                var schemaToInclude = this.Target.GetDefaultSchema();
                var tables = base.GetTargetTables();

                return tables.Where(x => x.Schema == schemaToInclude);
            }
        }
    }
}
