//
//  SchemaAwareDbContextService.cs
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

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Remora.EntityFrameworkCore.Modular.Services
{
    /// <summary>
    /// Serves functionality for schema-aware database contexts.
    /// </summary>
    [PublicAPI]
    public sealed class SchemaAwareDbContextService
    {
        /// <summary>
        /// Configures the options of a schema-aware database context.
        /// </summary>
        /// <param name="optionsBuilder">The options builder to configure.</param>
        [PublicAPI]
        public void ConfigureSchemaAwareContext([NotNull] DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IMigrationsModelDiffer, SchemaAwareMigrationsModelDiffer>();
        }
    }
}
