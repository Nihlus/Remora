//
//  IMigratablePlugin.cs
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
using Remora.Results;

namespace Remora.Plugins.Abstractions;

/// <summary>
/// Represents the public API of a plugin supporting some type of migration of its persistent store.
/// </summary>
[PublicAPI]
public interface IMigratablePlugin : IPluginDescriptor
{
    /// <summary>
    /// Performs any migrations required by the plugin.
    /// </summary>
    /// <param name="serviceProvider">The available services.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result> MigratePluginAsync(IServiceProvider serviceProvider);

    /// <summary>
    /// Determines whether the persistent store of the plugin has been created.
    /// </summary>
    /// <param name="serviceProvider">The available services.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<bool> HasCreatedPersistentStoreAsync(IServiceProvider serviceProvider);
}
