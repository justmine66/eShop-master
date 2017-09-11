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

namespace Ordering.API.Application.DomainEventHandlers.OrderPaid
{
    /// <summary>
    /// 订单状态改变成已支付领域事件处理
    /// </summary>
    public class OrderStatusChangedToPaidDomainEventHandler
        : IAsyncNotificationHandler<OrderStatusChangedToPaidDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public OrderStatusChangedToPaidDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(OrderStatusChangedToPaidDomainEvent @event)
        {
            _logger.CreateLogger(nameof(OrderStatusChangedToPaidDomainEventHandler))
                .LogTrace($"Order with Id: {@event.OrderId} has been successfully updated with " +
                          $"a status order id: {OrderStatus.Paid.Id}");

            var orderStockList = @event.OrderItems
                .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.GetUnits()));

            var integrationEvent = new OrderStatusChangedToPaidIntegrationEvent(@event.OrderId, orderStockList);
            await this._orderingIntegrationEventService.PublishThroughEventBusAsync(integrationEvent);
        }
    }
}
