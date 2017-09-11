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

namespace Ordering.API.Application.DomainEventHandlers.OrderGracePeriodConfirmed
{
    /// <summary>
    /// 订单状态改变成等待验证领域事件处理
    /// </summary>
    public class OrderStatusChangedToAwaitingValidationDomainEventHandler
        : IAsyncNotificationHandler<OrderStatusChangedToAwaitingValidationDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILoggerFactory _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public OrderStatusChangedToAwaitingValidationDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(OrderStatusChangedToAwaitingValidationDomainEvent @event)
        {
            var orderStockList = @event.OrderItems
                .Select(o => new OrderStockItem(o.ProductId, o.GetUnits()));

            var integrationEvent = new OrderStatusChangedToAwaitingValidationIntegrationEvent(@event.OrderId, orderStockList);
            await this._orderingIntegrationEventService.PublishThroughEventBusAsync(integrationEvent);

            _logger.CreateLogger(nameof(OrderStatusChangedToAwaitingValidationDomainEvent))
             .LogTrace($"Order with Id: {@event.OrderId} has been successfully updated with " +
                       $"a status order id: {OrderStatus.AwaitingValidation.Id}");
        }
    }
}
