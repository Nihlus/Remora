using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remora.Plugins.Abstractions;
using Remora.Plugins.Abstractions.Attributes;
using Remora.Templates.Plugin;

[assembly: RemoraPlugin(typeof(TemplatePlugin))]

namespace Remora.Templates.Plugin
{
    /// <summary>
    /// Describes the plugin.
    /// </summary>
    public sealed class TemplatePlugin : PluginDescriptor
    {
        /// <inheritdoc />
        public override string Name => "Template";

        /// <inheritdoc />
        public override string Description => "A template plugin.";

        /// <inheritdoc />
        public override void ConfigureServices(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Task<bool> InitializeAsync(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }
}
