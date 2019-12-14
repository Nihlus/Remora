//
//  LoadPluginResult.cs
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
using JetBrains.Annotations;
using Remora.Plugins.Abstractions;
using Remora.Results;

namespace Remora.Plugins.Results
{
    /// <summary>
    /// Represents an attempt to lead a plugin.
    /// </summary>
    [PublicAPI]
    public sealed class LoadPluginResult : ResultBase<LoadPluginResult>
    {
        /// <summary>
        /// Holds the actual plugin value.
        /// </summary>
        private IPluginDescriptor? _plugin;

        /// <summary>
        /// Gets the plugin that was loaded.
        /// </summary>
        [NotNull]
        public IPluginDescriptor Plugin
        {
            get
            {
                if (!this.IsSuccess || _plugin is null)
                {
                    throw new InvalidOperationException("The result does not contain a valid value.");
                }

                return _plugin;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPluginResult"/> class.
        /// </summary>
        private LoadPluginResult([NotNull] IPluginDescriptor plugin)
        {
            _plugin = plugin;
        }

        /// <inheritdoc cref="ResultBase{TResultType}(string,Exception)"/>
        [UsedImplicitly]
        private LoadPluginResult
        (
            string? errorReason,
            Exception? exception = null
        )
            : base(errorReason, exception)
        {
        }

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <param name="plugin">The plugin that was initialized.</param>
        /// <returns>A successful result.</returns>
        [PublicAPI, Pure, NotNull]
        public static LoadPluginResult FromSuccess([NotNull] IPluginDescriptor plugin)
        {
            return new LoadPluginResult(plugin);
        }
    }
}
