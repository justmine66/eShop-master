using Catalog.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents.EventHandling
{
    /// <summary>
    /// 产品价格已改变事件的事件处理
    /// </summary>
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        private readonly ILogger<ProductPriceChangedIntegrationEventHandler> _logger;//日志记录器
        public ProductPriceChangedIntegrationEventHandler(ILogger<ProductPriceChangedIntegrationEventHandler> logger)
        {
            _logger = logger ??
               throw new ArgumentNullException(nameof(logger));//日志记录器
        }
        public Task HandleAsync(ProductPriceChangedIntegrationEvent @event)
        {
            _logger.LogCritical($"事件标识:{@event.Id};产品标识:{@event.ProductId};已调用产品价格已改变事件的事件处理.");
            return Task.CompletedTask;
        }
    }
}
