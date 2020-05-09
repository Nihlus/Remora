//
//  TemplatePlugin.cs
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
