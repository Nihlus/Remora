//
//  ContinuousBehaviour.cs
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
using System.Transactions;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Results;

namespace Remora.Behaviours.Bases
{
    /// <summary>
    /// Abstract base class for a behaviour that continuously performs an action.
    /// </summary>
    /// <typeparam name="TBehaviour">The inheriting behaviour.</typeparam>
    [PublicAPI]
    public abstract class ContinuousBehaviour<TBehaviour> : BehaviourBase<TBehaviour>
        where TBehaviour : ContinuousBehaviour<TBehaviour>
    {
        /// <summary>
        /// Gets or sets the cancellation source for the continuous action task.
        /// </summary>
        private CancellationTokenSource CancellationSource { get; set; }

        /// <summary>
        /// Gets or sets the continuous action task.
        /// </summary>
        private Task ContinuousActionTask { get; set; }

        /// <summary>
        /// Gets the delay between ticks.
        /// </summary>
        protected virtual TimeSpan TickDelay => TimeSpan.FromSeconds(1);

        /// <summary>
        /// Gets a value indicating whether a transaction should be created for each tick. Defaults to true.
        /// </summary>
        protected virtual bool UseTransaction => true;

        /// <summary>
        /// Gets the timeout that should be used for the tick transaction, if one is created. Defaults to a
        /// <see cref="IsolationLevel.Serializable"/> transaction with a timeout of
        /// <see cref="TransactionManager.DefaultTimeout"/>.
        /// </summary>
        protected virtual TransactionOptions TransactionOptions { get; } = new TransactionOptions
        {
            Timeout = TransactionManager.DefaultTimeout,
            IsolationLevel = IsolationLevel.Serializable
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousBehaviour{TBehaviour}"/> class.
        /// </summary>
        /// <param name="serviceScope">The service scope of the behaviour.</param>
        /// <param name="logger">The logging instance for this type.</param>
        [PublicAPI]
        protected ContinuousBehaviour
        (
            IServiceScope serviceScope,
            ILogger<TBehaviour> logger
        )
            : base(serviceScope, logger)
        {
            this.CancellationSource = new CancellationTokenSource();
            this.ContinuousActionTask = Task.CompletedTask;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousBehaviour{TBehaviour}"/> class.
        /// </summary>
        /// <param name="serviceScope">The service scope of the behaviour.</param>
        [PublicAPI]
        protected ContinuousBehaviour
        (
            IServiceScope serviceScope
        )
            : base(serviceScope)
        {
            this.CancellationSource = new CancellationTokenSource();
            this.ContinuousActionTask = Task.CompletedTask;
        }

        /// <summary>
        /// Implements the body that should run on each tick of the behaviour. Usually, having some sort of delay in
        /// this method takes strain off of the system.
        ///
        /// This method takes part in a transaction scope for the duration of the tick. If the tick fails for any
        /// reason (be that an exception or a user-submitted error), the transaction is rolled back. Normally, this does
        /// not do anything at all, and a background infrastructure (such as a database provider) needs to make use of
        /// the transaction to commit or roll back its changes.
        /// </summary>
        /// <param name="ct">The cancellation token for the behaviour.</param>
        /// <param name="tickServices">The services available during the tick.</param>
        /// <returns>An operation result which may or may not have succeeded.</returns>
        protected abstract Task<OperationResult> OnTickAsync(CancellationToken ct, IServiceProvider tickServices);

        /// <summary>
        /// Continuously runs <see cref="OnTickAsync"/> until the behaviour stops.
        /// </summary>
        /// <param name="ct">The cancellation token for the behaviour.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task RunContinuousActionAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var tickScope = this.Services.CreateScope();
                    try
                    {
                        TransactionScope? transactionScope = null;
                        if (this.UseTransaction)
                        {
                            transactionScope = new TransactionScope
                            (
                                TransactionScopeOption.Required,
                                this.TransactionOptions,
                                TransactionScopeAsyncFlowOption.Enabled
                            );
                        }

                        var operationResult = await OnTickAsync(ct, tickScope.ServiceProvider);

                        if (operationResult.IsSuccess)
                        {
                            transactionScope?.Complete();
                        }

                        transactionScope?.Dispose();
                    }
                    finally
                    {
                        if (tickScope is IAsyncDisposable asyncDisposable)
                        {
                            await asyncDisposable.DisposeAsync();
                        }
                        else
                        {
                            tickScope.Dispose();
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    this.Log.LogDebug("Cancellation requested in continuous action - terminating.");
                    return;
                }
                catch (Exception e)
                {
                    // Nom nom nom
                    this.Log.LogError(e, "Error in behaviour tick.");
                }

                await Task.Delay(this.TickDelay, ct);
            }
        }

        /// <inheritdoc/>
        /// <remarks>You must call this base implementation in any derived methods.</remarks>
        protected override Task OnStartingAsync()
        {
            this.CancellationSource = new CancellationTokenSource();
            this.ContinuousActionTask = Task.Run(() => RunContinuousActionAsync(this.CancellationSource.Token));

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        /// <remarks>You must call this base implementation in any derived methods.</remarks>
        protected override async Task OnStoppingAsync()
        {
            this.CancellationSource.Cancel();
            await this.ContinuousActionTask;

            this.CancellationSource.Dispose();
        }
    }
}
