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

using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Remora.Behaviours.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="IServiceCollection"/> class.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a behaviour to the service collection. Behaviours are added as singleton services.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <typeparam name="TBehaviour">The behaviour type.</typeparam>
        /// <returns>The service collection, with the behaviour.</returns>
        public static IServiceCollection AddBehaviour<TBehaviour>
        (
            this IServiceCollection serviceCollection
        )
            where TBehaviour : class, IBehaviour
        {
            if (serviceCollection.Any(d => d.ServiceType == typeof(TBehaviour)))
            {
                // already added
                return serviceCollection;
            }

            serviceCollection.AddSingleton<TBehaviour>();
            serviceCollection.AddSingleton<IBehaviour, TBehaviour>(s => s.GetRequiredService<TBehaviour>());
            return serviceCollection;
        }
    }
}
