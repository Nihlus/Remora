//
//  IPluginDescriptor.cs
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
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Remora.Results;

namespace Remora.Plugins.Abstractions;

/// <summary>
/// Represents the public API for a plugin.
/// </summary>
[PublicAPI]
public interface IPluginDescriptor
{
    /// <summary>
    /// Gets the name of the plugin. This name should be unique.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the plugin.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Configures services provided by the plugin in the application's service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    void ConfigureServices(IServiceCollection serviceCollection);

    /// <summary>
    /// Performs any post-registration initialization required by the plugin.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>true if the plugin could successfully initialize itself; otherwise, false.</returns>
    ValueTask<Result> InitializeAsync(IServiceProvider serviceProvider);
}
