using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSys.SharedKernel.IntegrationEvents
{
    public class DelegateBackgroundService : BackgroundService
    {
        private readonly Func<CancellationToken, Task> _work;
        public DelegateBackgroundService(Func<CancellationToken, Task> work) => _work = work;
        protected override Task ExecuteAsync(CancellationToken stoppingToken) => _work(stoppingToken);
    }
}
