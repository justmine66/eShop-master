using Catalog.API.Infrastructure;
using Catalog.API.IntegrationEvents.Events;
using EventBus.Abstractions;
using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents.EventHandling
{
    /// <summary>
    /// 订单状态变更为等待验证事件处理
    /// </summary>
    public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler :
        IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>
    {
        private readonly CatalogContext _catalogContext;
        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;

        public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(CatalogContext catalogContext,
            ICatalogIntegrationEventService catalogIntegrationEventService)
        {
            _catalogContext = catalogContext;
            _catalogIntegrationEventService = catalogIntegrationEventService;
        }

        public async Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent @event)
        {
            var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();
            foreach (var orderStockItem in @event.OrderStockItems)
            {
                var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);
                var hasStock = catalogItem.AvailableStock >= orderStockItem.Units;
                var confiredOrderStockItem = new ConfirmedOrderStockItem(catalogItem.Id, hasStock);

                confirmedOrderStockItems.Add(confiredOrderStockItem);
            }

            IntegrationEvent confirmedIntegrationEvent = confirmedOrderStockItems.Any(c => c.HasStock)
                ? new OrderStockConfirmedIntegrationEvent(@event.OrderId) as IntegrationEvent
                : new OrderStockRejectedIntegrationEvent(@event.OrderId, confirmedOrderStockItems) as IntegrationEvent;

            await _catalogIntegrationEventService.PublishThroughEventBusAsync(confirmedIntegrationEvent);
            await _catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(confirmedIntegrationEvent);
        }

    }
}
