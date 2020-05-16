//
//  HostedRemoraService.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Behaviours.Services;
using Remora.Plugins.Abstractions;
using Remora.Plugins.Services;

namespace Remora.Hosting
{
    /// <summary>
    /// Acts as a base class for hosted remora services.
    /// </summary>
    /// <typeparam name="THostedRemoraService">The implementing Remora service.</typeparam>
    [PublicAPI]
    public abstract class HostedRemoraService<THostedRemoraService> : IHostedService
        where THostedRemoraService : HostedRemoraService<THostedRemoraService>
    {
        [PublicAPI, NotNull]
        private readonly PluginService _pluginService;

        [PublicAPI, NotNull]
        private readonly BehaviourService _behaviours;

        /// <summary>
        /// Gets the available services.
        /// </summary>
        [PublicAPI, NotNull]
        protected IServiceProvider Services { get; }

        /// <summary>
        /// Gets the application lifetime.
        /// </summary>
        [PublicAPI, NotNull]
        protected IHostApplicationLifetime Lifetime { get; }

        /// <summary>
        /// Gets the logging instance for this service.
        /// </summary>
        [PublicAPI, NotNull]
        protected ILogger<THostedRemoraService> Log { get; }

        /// <summary>
        /// Gets the host environment for this service.
        /// </summary>
        [PublicAPI, NotNull]
        protected IHostEnvironment HostEnvironment { get; }

        /// <summary>
        /// Gets the host configuration for this service.
        /// </summary>
        [PublicAPI, NotNull]
        protected IConfiguration HostConfiguration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostedRemoraService{TBotService}"/> class.
        /// </summary>
        /// <param name="pluginService">The plugin service.</param>
        /// <param name="behaviourService">The behaviour service.</param>
        /// <param name="hostConfiguration">The host configuration.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="log">The logging instance.</param>
        /// <param name="applicationLifetime">The application lifetime.</param>
        /// <param name="services">The available services.</param>
        protected HostedRemoraService
        (
            PluginService pluginService,
            BehaviourService behaviourService,
            IConfiguration hostConfiguration,
            IHostEnvironment hostEnvironment,
            ILogger<THostedRemoraService> log,
            IHostApplicationLifetime applicationLifetime,
            IServiceProvider services
        )
        {
            _pluginService = pluginService;
            _behaviours = behaviourService;

            this.HostConfiguration = hostConfiguration;
            this.Log = log;
            this.Lifetime = applicationLifetime;
            this.Services = services;
            this.HostEnvironment = hostEnvironment;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!await InitializePluginsAsync())
            {
                this.Log.LogError("Failed to initialize the available plugins.");

                // Plugin failures means we won't continue
                this.Lifetime.StopApplication();

                return;
            }

            // Let inheriting classes perform their startup procedures
            await OnStartingAsync(cancellationToken);

            await _behaviours.StartBehavioursAsync();
        }

        /// <summary>
        /// Perform startup procedures in inheriting classes.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task OnStartingAsync(CancellationToken cancellationToken);

        /// <inheritdoc />
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            this.Log.LogInformation("Stopping behaviours...");
            await _behaviours.StopBehavioursAsync();
        }

        private async Task<bool> InitializePluginsAsync()
        {
            var plugins = _pluginService.LoadAvailablePlugins().ToList();

            // Create plugin databases
            foreach (var plugin in plugins)
            {
                if (!(plugin is IMigratablePlugin migratablePlugin))
                {
                    continue;
                }

                if (await migratablePlugin.HasCreatedPersistentStoreAsync(this.Services))
                {
                    continue;
                }

                if (await migratablePlugin.MigratePluginAsync(this.Services))
                {
                    continue;
                }

                this.Log.LogWarning
                (
                    $"The plugin \"{plugin.Name}\" (v{plugin.Version}) failed to create its persistent store."
                );

                return false;
            }

            // Then, run migrations in reverse
            foreach (var plugin in plugins.AsEnumerable().Reverse())
            {
                if (!(plugin is IMigratablePlugin migratablePlugin))
                {
                    continue;
                }

                if (await migratablePlugin.MigratePluginAsync(this.Services))
                {
                    continue;
                }

                this.Log.LogWarning
                (
                    $"The plugin \"{plugin.Name}\" (v{plugin.Version}) failed to migrate its database."
                );

                return false;
            }

            foreach (var plugin in plugins)
            {
                if (await plugin.InitializeAsync(this.Services))
                {
                    continue;
                }

                this.Log.LogWarning
                (
                    $"The plugin \"{plugin.Name}\" (v{plugin.Version}) failed to initialize."
                );

                return false;
            }

            return true;
        }
    }
}
