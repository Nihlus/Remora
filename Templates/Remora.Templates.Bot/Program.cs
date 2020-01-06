using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Behaviours.Services;
using Remora.Plugins.Services;
using Remora.Templates.Bot.Services;

namespace Remora.Templates.Bot
{
    internal class Program
    {
        public static async Task Main()
        {
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(ConfigureLogging);

            var host = hostBuilder.Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation($"Running on {RuntimeInformation.FrameworkDescription}");

            await host.RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var pluginService = new PluginService();

            services
                .AddHostedService<TemplateBotService>()
                .AddSingleton<BehaviourService>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<BaseSocketClient>(s => s.GetRequiredService<DiscordSocketClient>())
                .AddSingleton<IDiscordClient>(s => s.GetRequiredService<DiscordSocketClient>())
                .AddSingleton(pluginService);

            var plugins = pluginService.LoadAvailablePlugins();
            foreach (var plugin in plugins)
            {
                plugin.ConfigureServices(services);
            }
        }

        private static void ConfigureLogging(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddConsole();
        }
    }
}
