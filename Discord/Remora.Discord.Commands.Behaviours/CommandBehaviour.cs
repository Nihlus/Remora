//
//  CommandBehaviour.cs
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Discord.Behaviours;

namespace Remora.Discord.Commands.Behaviours
{
    /// <summary>
    /// Acts as a behaviour for invoking commands, and logging their results.
    /// </summary>
    [PublicAPI]
    public class CommandBehaviour : ClientEventBehaviour<CommandBehaviour>
    {
        private readonly CommandService _commands;

        private readonly List<Func<SocketCommandContext, Task<bool>>> _commandFilters;

        /// <summary>
        /// Gets the prefix character for commands.
        /// </summary>
        protected virtual char CommandPrefix { get; } = '!';

        /// <summary>
        /// Gets a value indicating whether mentions of the bot should be treated the same as having a command prefix.
        /// </summary>
        protected virtual bool TreatMentionsAsCommands { get; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBehaviour"/> class.
        /// </summary>
        /// <param name="client">The discord client.</param>
        /// <param name="serviceScope">The service scope in use.</param>
        /// <param name="logger">The logging instance for this type.</param>
        /// <param name="commands">The command service.</param>
        public CommandBehaviour
        (
            DiscordSocketClient client,
            IServiceScope serviceScope,
            ILogger<CommandBehaviour> logger,
            CommandService commands
        )
            : base(client, serviceScope, logger)
        {
            _commands = commands;
            _commandFilters = new List<Func<SocketCommandContext, Task<bool>>>();
        }

        /// <inheritdoc />
        protected override async Task OnStartingAsync()
        {
            await base.OnStartingAsync();
            await ConfigureFiltersAsync(_commandFilters);
        }

        /// <summary>
        /// Configures the command filters in use by the behaviour. A filter is a function that determine whether a
        /// message with a command prefix should be handled by the bot - a return value of false indicates that it
        /// should not pass.
        ///
        /// By default, this method adds a set of standard filters to the list.
        /// </summary>
        /// <param name="commandFilters">The filter list to configure.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task ConfigureFiltersAsync(List<Func<SocketCommandContext, Task<bool>>> commandFilters)
        {
            _commandFilters.Add(StandardCommandFilters.IsUserAsync);
            _commandFilters.Add(c => StandardCommandFilters.IsSufficientlyLong(c));
            _commandFilters.Add(StandardCommandFilters.ContainsAtLeastOneLetterOrDigit);

            return Task.CompletedTask;
        }

        /// <summary>
        /// User-configurable callback that executes before a command is executed, but after it has passed all
        /// configured filters.
        /// </summary>
        /// <param name="context">The command context.</param>
        /// <param name="commandStart">The start of the command within the message.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task BeforeCommandAsync
        (
            SocketCommandContext context,
            int commandStart
        )
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// User-configurable callback that executes after a command is executed, regardless of whether it succeeded or
        /// not.
        /// </summary>
        /// <param name="context">The command context.</param>
        /// <param name="commandStart">The start of the command within the message.</param>
        /// <param name="result">The result of the command.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task AfterCommandAsync
        (
            SocketCommandContext context,
            int commandStart,
            ExecuteResult result
        )
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// User-configurable callback that runs if a command fails. The default implementation logs exceptions, and
        /// discards everything else.
        /// </summary>
        /// <param name="context">The command context.</param>
        /// <param name="commandStart">The start of the command within the message.</param>
        /// <param name="result">The result of the command.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual Task OnCommandFailedAsync
        (
            SocketCommandContext context,
            int commandStart,
            ExecuteResult result
        )
        {
            switch (result.Error)
            {
                case CommandError.Exception:
                {
                    this.Log.LogError(result.Exception, "Caught exception in command.");
                    break;
                }
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected sealed override async Task MessageUpdated
        (
            Cacheable<IMessage, ulong> oldMessage,
            SocketMessage updatedMessage,
            ISocketMessageChannel messageChannel
        )
        {
            // Ignore all changes except text changes
            var isTextUpdate = updatedMessage.EditedTimestamp.HasValue &&
                               updatedMessage.EditedTimestamp.Value > DateTimeOffset.Now - TimeSpan.FromMinutes(1);

            if (!isTextUpdate)
            {
                return;
            }

            await MessageReceived(updatedMessage);
        }

        /// <inheritdoc />
        protected sealed override async Task MessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message))
            {
                return;
            }

            var foundCommandStart = false;
            var argumentPos = 0;
            if (this.TreatMentionsAsCommands)
            {
                foundCommandStart = message.HasMentionPrefix(this.Client.CurrentUser, ref argumentPos);
            }

            if (!foundCommandStart)
            {
                if (!message.HasCharPrefix(this.CommandPrefix, ref argumentPos))
                {
                    return;
                }
            }

            var context = new SocketCommandContext(this.Client, message);

            // Run all configured filters against the command
            foreach (var filter in _commandFilters)
            {
                if (!await filter(context))
                {
                    return;
                }
            }

            await BeforeCommandAsync(context, argumentPos);

            // Create a service scope for this command
            using (var scope = this.Services.CreateScope())
            {
                var result = await _commands.ExecuteAsync(context, argumentPos, scope.ServiceProvider);
                if (!(result is ExecuteResult executeResult))
                {
                    this.Log.LogError
                    (
                        "The returned result from a command execution was not an ExecuteResult. Post-execution " +
                        "callbacks will not run. This is a bug in the library, or Discord.Net."
                    );

                    return;
                }

                if (!result.IsSuccess)
                {
                    await OnCommandFailedAsync(context, argumentPos, executeResult);
                }

                await AfterCommandAsync(context, argumentPos, executeResult);
            }
        }
    }
}
