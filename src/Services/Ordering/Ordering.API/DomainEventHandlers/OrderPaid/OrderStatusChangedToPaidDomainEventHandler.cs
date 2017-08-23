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

namespace Ordering.API.DomainEventHandlers.OrderPaid
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
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(OrderStatusChangedToPaidDomainEvent domainEvent)
        {
            this._logger.CreateLogger(nameof(OrderStatusChangedToPaidDomainEventHandler))
                .LogTrace($"订单 {domainEvent.OrderId} 状态已成功改变为:{OrderStatus.Paid.Id}{OrderStatus.Paid.Name}");

            var orderStockList = domainEvent.OrderItems
                .Select(orderItem => { return new OrderStockItem(orderItem.ProductId, orderItem.GetUnits()); });

            var integrationEvent = new OrderStatusChangedToPaidIntegrationEvent(domainEvent.OrderId, orderStockList);
            await this._orderingIntegrationEventService.PublishThroughEventBusAsync(integrationEvent);
        }
    }
}
