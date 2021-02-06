//
//  DelayedAction.cs
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
using Remora.Results;

namespace Remora.Behaviours.Services
{
    /// <summary>
    /// Represents a wrapped delayed action.
    /// </summary>
    public class DelayedAction
    {
        /// <summary>
        /// Gets the delay task until the action should take place.
        /// </summary>
        public Task Delay { get; }

        /// <summary>
        /// Gets the action that should take place.
        /// </summary>
        public Func<Task<Result>> Action { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedAction"/> class.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="action">The action.</param>
        public DelayedAction(TimeSpan delay, Func<Task<Result>> action)
        {
            this.Delay = Task.Delay(delay);
            this.Action = action;
        }
    }
}
