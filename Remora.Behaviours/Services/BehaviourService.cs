//
//  BehaviourService.cs
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
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Remora.Behaviours.Services
{
    /// <summary>
    /// This class manages the access to and lifetime of registered behaviours.
    /// </summary>
    [PublicAPI]
    public sealed class BehaviourService
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviourService"/> class.
        /// </summary>
        /// <param name="services">The services of the application.</param>
        public BehaviourService(IServiceProvider services)
        {
            _services = services;
        }

        /// <summary>
        /// Starts all registered behaviours.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [PublicAPI]
        public async Task StartBehavioursAsync()
        {
            var behaviours = _services.GetServices<IBehaviour>();
            await Task.WhenAll(behaviours.Select(b => b.StartAsync()));
        }

        /// <summary>
        /// Stops all registered behaviours.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [PublicAPI]
        public async Task StopBehavioursAsync()
        {
            var behaviours = _services.GetServices<IBehaviour>();
            await Task.WhenAll(behaviours.Select(b => b.StopAsync()));
        }
    }
}
