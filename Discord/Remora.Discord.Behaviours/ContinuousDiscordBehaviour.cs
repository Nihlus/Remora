//
//  ContinuousDiscordBehaviour.cs
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
using Discord;
using Discord.WebSocket;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Behaviours.Bases;

namespace Remora.Discord.Behaviours
{
    /// <summary>
    /// Represents a Discord-enabled continuous behaviour.
    /// </summary>
    /// <typeparam name="TBehaviour">The inheriting behaviour type.</typeparam>
    [PublicAPI]
    public abstract class ContinuousDiscordBehaviour<TBehaviour> : ContinuousBehaviour<TBehaviour>
        where TBehaviour : ContinuousBehaviour<TBehaviour>
    {
        /// <summary>
        /// Gets the Discord client in use.
        /// </summary>
        [PublicAPI]
        protected DiscordSocketClient Client { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousDiscordBehaviour{TBehaviour}"/> class.
        /// </summary>
        /// <param name="client">The Discord client.</param>
        /// <param name="serviceScope">The service scope in use.</param>
        /// <param name="logger">The logging instance for this type.</param>
        [PublicAPI]
        protected ContinuousDiscordBehaviour
        (
            DiscordSocketClient client,
            IServiceScope serviceScope,
            ILogger<TBehaviour> logger
        )
            : base(serviceScope, logger)
        {
            this.Client = client;
        }

        /// <inheritdoc />
        /// <remarks>You must call this base implementation in any derived methods.</remarks>
        protected override async Task OnStartingAsync()
        {
            while (this.Client.ConnectionState != ConnectionState.Connected)
            {
                // Give the client some time to start up
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            await base.OnStartingAsync();
        }
    }
}
