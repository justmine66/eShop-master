using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Ordering.API.Infrastructure.HostedServices
{
    public abstract class HostedService : IHostedService
    {
        private Task _executingTask;
        private CancellationTokenSource _cts;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            this._executingTask = this.ExecuteAsync(this._cts.Token);
            return this._executingTask.IsCompleted ? this._executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (this._executingTask==null)
            {
                return;
            }

            await Task.WhenAny(this._executingTask, Task.Delay(-1, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
