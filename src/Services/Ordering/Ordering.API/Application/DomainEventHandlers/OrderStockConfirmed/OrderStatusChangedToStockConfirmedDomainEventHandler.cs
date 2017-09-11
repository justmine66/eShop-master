using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents;
using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.DomainEventHandlers.OrderStockConfirmed
{
    /// <summary>
    /// 订单状态改变成库存确认领域事件处理
    /// </summary>
    public class OrderStatusChangedToStockConfirmedDomainEventHandler
        : IAsyncNotificationHandler<OrderStatusChangedToStockConfirmedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public OrderStatusChangedToStockConfirmedDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(OrderStatusChangedToStockConfirmedDomainEvent @event)
        {
            _logger.CreateLogger(nameof(OrderStatusChangedToStockConfirmedDomainEventHandler))
               .LogTrace($"Order with Id: {@event.OrderId} has been successfully updated with " +
                         $"a status order id: {OrderStatus.StockConfirmed.Id}");

            var integration = new OrderStatusChangedToStockConfirmedIntegrationEvent(@event.OrderId);
            await this._orderingIntegrationEventService.PublishThroughEventBusAsync(integration);
        }
    }
}
