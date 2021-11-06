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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
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
        /// <param name="commandBatchPreparerDependencies">The command batch preparer dependencies.</param>
        public SchemaAwareMigrationsModelDiffer
        (
            IRelationalTypeMappingSource typeMappingSource,
            IMigrationsAnnotationProvider migrationsAnnotations,
            IChangeDetector changeDetector,
            IUpdateAdapterFactory updateAdapterFactory,
            CommandBatchPreparerDependencies commandBatchPreparerDependencies
        )
            : base
            (
                typeMappingSource,
                migrationsAnnotations,
                changeDetector,
                updateAdapterFactory,
                commandBatchPreparerDependencies
            )
        {
        }

        /// <inheritdoc/>
        public override bool HasDifferences(IRelationalModel source, IRelationalModel target)
            => Diff(source, target, new SchemaAwareDiffContext(source, target)).Any();

        /// <inheritdoc/>
        public override IReadOnlyList<MigrationOperation> GetDifferences
        (
            IRelationalModel source,
            IRelationalModel target
        )
        {
            var diffContext = new SchemaAwareDiffContext(source, target);
            return Sort(Diff(source, target, diffContext), diffContext);
        }

        /// <inheritdoc/>
        protected override IEnumerable<MigrationOperation> Diff
        (
            IEnumerable<IForeignKeyConstraint> source,
            IEnumerable<IForeignKeyConstraint> target,
            DiffContext diffContext
        )
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
                    if (diffContext is not SchemaAwareDiffContext schemaDiffContext)
                    {
                        throw new InvalidOperationException();
                    }

                    if (s.Name != t.Name)
                    {
                        return false;
                    }

                    var sourceColumnNames = s.Columns.Select(sc => sc.Name);
                    var targetColumnNames = t.Columns.Select(p => c.FindSource(p)?.Name);
                    if (!sourceColumnNames.SequenceEqual(targetColumnNames))
                    {
                        return false;
                    }

                    var schemaToInclude = schemaDiffContext.Source?.Model.GetDefaultSchema();
                    if (schemaToInclude is null)
                    {
                        return false;
                    }

                    if (s.PrincipalTable.Schema == schemaToInclude &&
                        s.PrincipalTable.Schema != t.PrincipalTable.Schema)
                    {
                        return false;
                    }

                    var sourcePrincipalColumnNames = s.PrincipalColumns.Select(sc => sc.Name);
                    var targetPrincipalColumnNames = t.PrincipalColumns.Select(tc => c.FindSource(tc)?.Name).ToList();
                    if (targetPrincipalColumnNames.First() != null &&
                        !sourcePrincipalColumnNames.SequenceEqual(targetPrincipalColumnNames))
                    {
                        return false;
                    }

                    if (s.OnDeleteAction != t.OnDeleteAction)
                    {
                        return false;
                    }

                    return !HasDifferences
                    (
                        this.MigrationsAnnotations.ForRemove(s),
                        this.MigrationsAnnotations.ForRemove(t)
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
            public IRelationalModel? Source { get; }

            /// <summary>
            /// Gets the target model.
            /// </summary>
            public IRelationalModel? Target { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="SchemaAwareDiffContext"/> class.
            /// </summary>
            /// <param name="source">The source model.</param>
            /// <param name="target">The target model.</param>
            public SchemaAwareDiffContext(IRelationalModel? source, IRelationalModel? target)
            {
                this.Source = source;
                this.Target = target;
            }

            /// <inheritdoc/>
            public override T FindSource<T>(T target)
            {
                if (target is not ITable targetTable)
                {
                    return base.FindSource(target);
                }

                var schemaToInclude = this.Source?.Model.GetDefaultSchema();
                var source = base.FindSource(targetTable);
                if (source is null)
                {
                    return null!;
                }

                if (source.Schema == schemaToInclude)
                {
                    return (T)source;
                }

                // This method can return null, and should do so when it doesn't find anything
                return null!;
            }

            /// <inheritdoc/>
            public override T FindTarget<T>(T source)
            {
                if (source is not ITable sourceTable)
                {
                    return base.FindTarget(source);
                }

                var schemaToInclude = this.Target?.Model.GetDefaultSchema();
                var target = base.FindTarget(sourceTable);
                if (target is null)
                {
                    return null!;
                }

                if (target.Schema == schemaToInclude)
                {
                    return (T)target;
                }

                // This method can return null, and should do so when it doesn't find anything
                return null!;
            }
        }
    }
}
