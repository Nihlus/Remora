//
//  ServiceCollectionExtensions.cs
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Remora.EntityFrameworkCore.Modular.Services;

namespace Remora.EntityFrameworkCore.Modular.Extensions
{
    /// <summary>
    /// Extension methods for service collections.
    /// </summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures a pool of schema-aware database contexts.
        /// </summary>
        /// <param name="this">The service collection.</param>
        /// <param name="userConfigurationFunction">The user-exposed configuration function.</param>
        /// <typeparam name="TContext">The context type to add.</typeparam>
        /// <returns>The service collection, with the pool.</returns>
        [PublicAPI, NotNull]
        public static IServiceCollection AddSchemaAwareDbContextPool<TContext>
        (
            [NotNull] this IServiceCollection @this,
            [NotNull] Action<DbContextOptionsBuilder> userConfigurationFunction
        )
            where TContext : SchemaAwareDbContext
        {
            @this.AddDbContextPool<TContext>
            (
                (serviceProvider, optionsBuilder) =>
                {
                    var schemaAwareDbContextService = serviceProvider.GetRequiredService<SchemaAwareDbContextService>();
                    schemaAwareDbContextService.ConfigureSchemaAwareContext(optionsBuilder);

                    userConfigurationFunction(optionsBuilder);
                }
            );

            return @this;
        }
    }
}
