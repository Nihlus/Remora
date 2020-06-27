//
//  DelayedActionService.cs
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
using System.Collections.Concurrent;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Results;

namespace Remora.Behaviours.Services
{
    /// <summary>
    /// Handles queueing of things to do at a later time.
    /// </summary>
    [PublicAPI]
    public class DelayedActionService
    {
        /// <summary>
        /// Gets the currently running delay tasks.
        /// </summary>
        [NotNull]
        internal ConcurrentQueue<DelayedAction> RunningActions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedActionService"/> class.
        /// </summary>
        [PublicAPI]
        public DelayedActionService()
        {
            this.RunningActions = new ConcurrentQueue<DelayedAction>();
        }

        /// <summary>
        /// Schedules an action to be performed at an arbitrary time in the future.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="delay">The time to delay its execution.</param>
        [PublicAPI]
        public void DelayUntil([NotNull] Func<Task> action, TimeSpan delay)
        {
            async Task<OperationResult> WrappedAction()
            {
                try
                {
                    await action();
                }
                catch (Exception e)
                {
                    return OperationResult.FromError(e);
                }

                return OperationResult.FromSuccess();
            }

            this.RunningActions.Enqueue(new DelayedAction(delay, WrappedAction));
        }

        /// <summary>
        /// Schedules an action to be performed at an arbitrary time in the future.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="delay">The time to delay its execution.</param>
        [PublicAPI]
        public void DelayUntil([NotNull] Func<Task<OperationResult>> action, TimeSpan delay)
        {
            this.RunningActions.Enqueue(new DelayedAction(delay, action));
        }
    }
}
