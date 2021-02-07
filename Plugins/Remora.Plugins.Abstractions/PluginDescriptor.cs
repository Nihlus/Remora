//
//  PluginDescriptor.cs
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
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Remora.Results;

namespace Remora.Plugins.Abstractions
{
    /// <summary>
    /// Acts as a base class for plugin descriptors.
    /// </summary>
    [PublicAPI]
    public abstract class PluginDescriptor : IPluginDescriptor
    {
        /// <inheritdoc />
        [PublicAPI]
        public abstract string Name { get; }

        /// <inheritdoc />
        [PublicAPI]
        public abstract string Description { get; }

        /// <inheritdoc />
        [PublicAPI]
        public virtual Version Version => Assembly.GetAssembly(GetType())?.GetName().Version ?? new Version(1, 0, 0);

        /// <inheritdoc />
        [PublicAPI]
        public virtual void ConfigureServices(IServiceCollection serviceCollection)
        {
        }

        /// <inheritdoc />
        [PublicAPI]
        public virtual ValueTask<Result> InitializeAsync(IServiceProvider serviceProvider)
        {
            return new(Result.FromSuccess());
        }

        /// <inheritdoc/>
        public sealed override string ToString()
        {
            return this.Name;
        }
    }
}
