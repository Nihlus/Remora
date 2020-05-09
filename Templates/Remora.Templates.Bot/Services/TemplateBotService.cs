//
//  TemplateBotService.cs
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
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Behaviours.Services;
using Remora.Discord.Hosted;
using Remora.Plugins.Services;
using Remora.Results;

namespace Remora.Templates.Bot.Services
{
    /// <summary>
    /// Main service for the bot itself. Handles high-level functionality.
    /// </summary>
    public class TemplateBotService : HostedDiscordBotService<TemplateBotService>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateBotService"/> class.
        /// </summary>
        /// <param name="discordClient">The Discord client.</param>
        /// <param name="behaviourService">The behaviour service.</param>
        /// <param name="pluginService">The plugin service.</param>
        /// <param name="hostConfiguration">The host configuration.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="log">The logging instance.</param>
        /// <param name="applicationLifetime">The application lifetime.</param>
        /// <param name="services">The available services.</param>
        public TemplateBotService
        (
            DiscordSocketClient discordClient,
            PluginService pluginService,
            BehaviourService behaviourService,
            IConfiguration hostConfiguration,
            IHostEnvironment hostEnvironment,
            ILogger<TemplateBotService> log,
            IHostApplicationLifetime applicationLifetime,
            IServiceProvider services
        )
            : base
            (
                discordClient,
                pluginService,
                behaviourService,
                hostConfiguration,
                hostEnvironment,
                log,
                applicationLifetime,
                services
            )
        {
        }

        /// <inheritdoc/>
        protected override Task<RetrieveEntityResult<string>> GetTokenAsync()
        {
            throw new NotImplementedException();
        }
    }
}
