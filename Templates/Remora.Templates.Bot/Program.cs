//
//  Program.cs
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
    /// <summary>
    /// The main class of the program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
