//
//  BehaviourBase.cs
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

using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Remora.Behaviours
{
    /// <summary>
    /// Acts as a base class for behaviours in the client.
    /// </summary>
    /// <typeparam name="TBehaviour">The inheriting behaviour.</typeparam>
    public abstract class BehaviourBase<TBehaviour> : IBehaviour
        where TBehaviour : BehaviourBase<TBehaviour>
    {
        /// <summary>
        /// Gets the scope in which this behaviour lives.
        /// </summary>
        [NotNull]
        private IServiceScope ServiceScope { get; }

        /// <summary>
        /// Gets the logging instance for this behaviour.
        /// </summary>
        [PublicAPI, NotNull]
        protected ILogger Log { get; }

        /// <inheritdoc />
        [PublicAPI]
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviourBase{TBehaviour}"/> class.
        /// </summary>
        /// <param name="serviceScope">The service scope of the behaviour.</param>
        /// <param name="logger">The logging instance for this type.</param>
        [PublicAPI]
        protected BehaviourBase(IServiceScope serviceScope, ILogger<TBehaviour> logger)
        {
            this.ServiceScope = serviceScope;
            this.Log = logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviourBase{TBehaviour}"/> class.
        /// </summary>
        /// <param name="serviceScope">The service scope of the behaviour.</param>
        [PublicAPI]
        protected BehaviourBase(IServiceScope serviceScope)
        {
            this.ServiceScope = serviceScope;
            this.Log = NullLogger.Instance;
        }

        /// <inheritdoc/>
        [PublicAPI]
        public async Task StartAsync()
        {
            if (this.IsRunning)
            {
                return;
            }

            this.IsRunning = true;
            await OnStartingAsync();
        }

        /// <summary>
        /// User-implementable logic that runs during behaviour startup.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [PublicAPI, NotNull]
        protected virtual Task OnStartingAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        [PublicAPI]
        public async Task StopAsync()
        {
            if (!this.IsRunning)
            {
                return;
            }

            this.IsRunning = false;
            await OnStoppingAsync();
        }

        /// <summary>
        /// User-implementable logic that runs during behaviour shutdown.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [PublicAPI, NotNull]
        protected virtual Task OnStoppingAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        [PublicAPI]
        public virtual void Dispose()
        {
            this.ServiceScope.Dispose();
        }
    }
}
