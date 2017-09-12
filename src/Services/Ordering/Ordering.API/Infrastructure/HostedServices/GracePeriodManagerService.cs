using EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.API.Application.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Ordering.API.Infrastructure.HostedServices
{
    public class GracePeriodManagerService : HostedService
    {
        private readonly OrderingSettings _settings;
        private readonly ILogger<GracePeriodManagerService> _logger;
        private readonly IEventBus _eventBus;

        public GracePeriodManagerService(IOptions<OrderingSettings> settings,
            IEventBus eventBus,
            ILogger<GracePeriodManagerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                this.CheckConfirmedGracePeriodOrders();

                await Task.Delay(this._settings.CheckUpdateTime, cancellationToken);

                continue;
            }
        }
        //检查宽限期内已确认的订单集
        private void CheckConfirmedGracePeriodOrders()
        {
            _logger.LogDebug($"正在检查已确认的订单");

            var orderIds = this.GetConfirmedGracePeriodOrders();

            foreach (var orderId in orderIds)
            {
                var gracePeriodConfirmedIntegrationEvent = new GracePeriodConfirmedIntegrationEvent(orderId);
                this._eventBus.Publish(gracePeriodConfirmedIntegrationEvent);
            }
        }
        //获取宽限期内已确认的订单集
        private IEnumerable<int> GetConfirmedGracePeriodOrders()
        {
            IEnumerable<int> orderIds = new List<int>();

            using (var conn = new SqlConnection(this._settings.ConnectionString))
            {
                try
                {
                    conn.Open();
                    orderIds = conn.Query<int>(@"SELECT Id FROM [eShop.Services.OrderingDb].[ordering].[orders] 
                            WHERE DATEDIFF(minute, [OrderDate], GETDATE()) >= @GracePeriodTime
                            AND [OrderStatusId] = 1", 
                            new { GracePeriodTime = this._settings.CheckUpdateTime });
                }
                catch (SqlException exception)
                {
                    _logger.LogCritical($"FATAL ERROR: Database connections could not be opened: {exception.Message}");
                }
            }

            return orderIds;
        }
    }
}
