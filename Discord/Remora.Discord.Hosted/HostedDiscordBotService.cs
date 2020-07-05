//
//  HostedDiscordBotService.cs
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
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Behaviours.Services;
using Remora.Hosting;
using Remora.Plugins.Services;
using Remora.Results;

namespace Remora.Discord.Hosted
{
    /// <summary>
    /// Main service for Discord-based bots. Handles high-level functionality.
    /// </summary>
    /// <typeparam name="TDiscordBotService">The implementing service.</typeparam>
    [PublicAPI]
    public abstract class HostedDiscordBotService<TDiscordBotService> : HostedRemoraService<TDiscordBotService>
        where TDiscordBotService : HostedRemoraService<TDiscordBotService>
    {
        [PublicAPI]
        private readonly DiscordSocketClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostedDiscordBotService{TDiscordBotService}"/> class.
        /// </summary>
        /// <param name="discordClient">The Discord client.</param>
        /// <param name="pluginService">The plugin service.</param>
        /// <param name="behaviourService">The behaviour service.</param>
        /// <param name="hostConfiguration">The host configuration.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="log">The logging instance.</param>
        /// <param name="applicationLifetime">The application lifetime.</param>
        /// <param name="services">The available services.</param>
        protected HostedDiscordBotService
        (
            DiscordSocketClient discordClient,
            PluginService pluginService,
            BehaviourService behaviourService,
            IConfiguration hostConfiguration,
            IHostEnvironment hostEnvironment,
            ILogger<TDiscordBotService> log,
            IHostApplicationLifetime applicationLifetime,
            IServiceProvider services
        )
            : base
            (
                pluginService,
                behaviourService,
                hostConfiguration,
                hostEnvironment,
                log,
                applicationLifetime,
                services
            )
        {
            _client = discordClient;
            _client.Log += OnDiscordLogEvent;
        }

        /// <summary>
        /// Gets the login token used by the bot.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [PublicAPI, ItemNotNull]
        protected abstract Task<RetrieveEntityResult<string>> GetTokenAsync();

        /// <inheritdoc />
        protected sealed override async Task OnStartingAsync(CancellationToken cancellationToken)
        {
            var loginResult = await LoginAsync();
            if (!loginResult.IsSuccess)
            {
                this.Log.LogError(loginResult.Exception, loginResult.ErrorReason);

                // Login failures means we won't continue
                this.Lifetime.StopApplication();

                return;
            }

            await _client.StartAsync();
        }

        /// <inheritdoc />
        public sealed override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            await _client.LogoutAsync();
            await _client.StopAsync();
        }

        /// <summary>
        /// Saves log events from Discord using the configured method in the host.
        /// </summary>
        /// <param name="arg">The log message from Discord.</param>
        /// <returns>A completed task.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the log severity is not recognized.</exception>
        [PublicAPI]
        protected Task OnDiscordLogEvent(LogMessage arg)
        {
            var content = $"Discord log event: {arg.Message}";
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                {
                    this.Log.LogCritical(content, arg.Exception);
                    break;
                }
                case LogSeverity.Error:
                {
                    this.Log.LogError(content, arg.Exception);
                    break;
                }
                case LogSeverity.Warning:
                {
                    this.Log.LogWarning(content, arg.Exception);
                    break;
                }
                case LogSeverity.Verbose:
                case LogSeverity.Info:
                {
                    this.Log.LogInformation(content, arg.Exception);
                    break;
                }
                case LogSeverity.Debug:
                {
                    this.Log.LogDebug(content, arg.Exception);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs the bot into Discord.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task<ModifyEntityResult> LoginAsync()
        {
            var getTokenAsync = await GetTokenAsync();
            if (!getTokenAsync.IsSuccess)
            {
                return ModifyEntityResult.FromError(getTokenAsync);
            }

            var token = getTokenAsync.Entity.Trim();
            await _client.LoginAsync(TokenType.Bot, token);

            return ModifyEntityResult.FromSuccess();
        }
    }
}
