//
//  DelayedActionBehaviour.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Behaviours.Bases;
using Remora.Behaviours.Services;

namespace Remora.Behaviours
{
    /// <summary>
    /// Represents a behaviour that does things at a later date.
    /// </summary>
    [PublicAPI]
    public sealed class DelayedActionBehaviour : ContinuousBehaviour<DelayedActionBehaviour>
    {
        /// <summary>
        /// Gets the events that are currently running.
        /// </summary>
        [NotNull]
        private readonly DelayedActionService _delayedActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedActionBehaviour"/> class.
        /// </summary>
        /// <param name="serviceScope">The service scope in use.</param>
        /// <param name="logger">The logging instance for this type.</param>
        /// <param name="delayedActions">The do-later service.</param>
        [PublicAPI]
        public DelayedActionBehaviour
        (
            [NotNull] IServiceScope serviceScope,
            [NotNull] ILogger<DelayedActionBehaviour> logger,
            [NotNull] DelayedActionService delayedActions
        )
            : base(serviceScope, logger)
        {
            _delayedActions = delayedActions;
        }

        /// <inheritdoc />
        protected override async Task OnTickAsync(CancellationToken ct)
        {
            if (_delayedActions.RunningTimeouts.TryDequeue(out var timeout))
            {
                if (timeout.IsCompleted)
                {
                    try
                    {
                        await timeout;

                        // Get and perform the actual task
                        var taskFactory = _delayedActions.ScheduledTasks[timeout];
                        await taskFactory();
                    }
                    catch (TaskCanceledException)
                    {
                        this.Log.LogDebug("Cancellation requested in delayed action - terminating.");
                        return;
                    }
                    catch (Exception e)
                    {
                        // Nom nom nom
                        this.Log.LogError(e, "Error in delayed action.");
                    }
                }
                else
                {
                    _delayedActions.RunningTimeouts.Enqueue(timeout);
                }
            }

            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200), ct);
            }
            catch (TaskCanceledException)
            {
                this.Log.LogDebug("Cancellation requested in delayed action - terminating.");
            }
        }
    }
}
