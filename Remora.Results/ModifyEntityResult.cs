//
//  ModifyEntityResult.cs
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
using JetBrains.Annotations;

namespace Remora.Results
{
    /// <summary>
    /// Encapsulates the result of an attempt to edit an entity.
    /// </summary>
    [PublicAPI]
    public sealed class ModifyEntityResult : ResultBase<ModifyEntityResult>
    {
        /// <summary>
        /// Gets a value indicating whether or not any entity was modified.
        /// </summary>
        [PublicAPI]
        public bool WasModified { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyEntityResult"/> class.
        /// </summary>
        /// <param name="wasModified">Whether the entity was modified.</param>
        private ModifyEntityResult(bool wasModified)
        {
            this.WasModified = wasModified;
        }

        /// <inheritdoc cref="ResultBase{TResultType}(string,Exception)"/>
        [UsedImplicitly]
        private ModifyEntityResult
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
        /// <param name="wasModified">Whether the entity was modified.</param>
        /// <returns>A successful result.</returns>
        [PublicAPI, Pure, NotNull]
        public static ModifyEntityResult FromSuccess(bool wasModified = true)
        {
            return new ModifyEntityResult(wasModified);
        }
    }
}
