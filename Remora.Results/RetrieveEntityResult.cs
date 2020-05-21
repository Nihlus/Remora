//
//  RetrieveEntityResult.cs
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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Remora.Results
{
    /// <summary>
    /// Represents an attempt to retrieve an entity from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to encapsulate.</typeparam>
    [PublicAPI]
    public sealed class RetrieveEntityResult<TEntity> : ResultBase<RetrieveEntityResult<TEntity>>
        where TEntity : notnull
    {
        /// <summary>
        /// Holds the actual entity value.
        /// </summary>
        [MaybeNull, AllowNull]
        private readonly TEntity _entity = default!;

        /// <summary>
        /// Gets the entity that was retrieved.
        /// </summary>
        [PublicAPI]
        [JetBrains.Annotations.NotNull]
        public TEntity Entity
        {
            get
            {
                if (!this.IsSuccess || _entity is null)
                {
                    throw new InvalidOperationException("The result does not contain a valid value.");
                }

                return _entity;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetrieveEntityResult{T}"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private RetrieveEntityResult(TEntity entity)
        {
            _entity = entity;
        }

        /// <inheritdoc cref="ResultBase{TResultType}(string,Exception)"/>
        [UsedImplicitly]
        private RetrieveEntityResult
        (
            string? errorReason,
            Exception? exception = null
        )
            : base(errorReason, exception)
        {
        }

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <param name="entity">The entity that was retrieved.</param>
        /// <returns>A successful result.</returns>
        [PublicAPI, Pure, JetBrains.Annotations.NotNull]
        public static RetrieveEntityResult<TEntity> FromSuccess([JetBrains.Annotations.NotNull] TEntity entity)
        {
            return new RetrieveEntityResult<TEntity>(entity);
        }

        /// <summary>
        /// Implicitly converts a compatible value to a successful result.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The successful result.</returns>
        [PublicAPI, Pure, JetBrains.Annotations.NotNull]
        public static implicit operator RetrieveEntityResult<TEntity>([JetBrains.Annotations.NotNull] TEntity entity)
        {
            return FromSuccess(entity);
        }
    }
}
