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
