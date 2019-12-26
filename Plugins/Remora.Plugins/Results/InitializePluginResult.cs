//
//  InitializePluginResult.cs
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
    /// Represents an attempt to initialize a plugin.
    /// </summary>
    [PublicAPI]
    public sealed class InitializePluginResult : ResultBase<InitializePluginResult>
    {
        /// <summary>
        /// Holds the actual plugin value.
        /// </summary>
        private IPluginDescriptor? _plugin;

        /// <summary>
        /// Gets the plugin that was initialized.
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
        /// Initializes a new instance of the <see cref="InitializePluginResult"/> class.
        /// </summary>
        private InitializePluginResult([NotNull] IPluginDescriptor plugin)
        {
            _plugin = plugin;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializePluginResult"/> class.
        /// </summary>
        /// <param name="plugin">The plugin that failed to initialize.</param>
        /// <param name="errorReason">The reason it failed to initialize.</param>
        /// <param name="exception">The exception that caused the failure, if any.</param>
        private InitializePluginResult
        (
            [NotNull] IPluginDescriptor plugin,
            string? errorReason,
            Exception? exception = null
        )
            : base(errorReason, exception)
        {
            _plugin = plugin;
        }

        /// <inheritdoc cref="ResultBase{TResultType}(string,Exception)"/>
        [UsedImplicitly]
        private InitializePluginResult
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
        public static InitializePluginResult FromSuccess([NotNull] IPluginDescriptor plugin)
        {
            return new InitializePluginResult(plugin);
        }

        /// <summary>
        /// Creates a new failed result.
        /// </summary>
        /// <param name="plugin">The plugin that failed to initialize.</param>
        /// <param name="errorReason">The reason the plugin failed to initialize.</param>
        /// <param name="exception">The exception that caused the failure, if any.</param>
        /// <returns>A failed result.</returns>
        [PublicAPI, Pure, NotNull]
        public static InitializePluginResult FromError
        (
            [NotNull] IPluginDescriptor plugin,
            [NotNull] string errorReason,
            Exception? exception = null
        )
        {
            return new InitializePluginResult(plugin, errorReason, exception);
        }
    }
}
