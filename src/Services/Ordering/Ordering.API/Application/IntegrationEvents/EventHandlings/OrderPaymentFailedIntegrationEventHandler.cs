using EventBus.Abstractions;
using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.IntegrationEvents.EventHandlings
{
    /// <summary>
    /// 订单支付失败集成事件处理
    /// </summary>
    public class OrderPaymentFailedIntegrationEventHandler :
        IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public OrderPaymentFailedIntegrationEventHandler(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }

        public async Task HandleAsync(OrderPaymentFailedIntegrationEvent @event)
        {
            var orderToUpdate = await this._orderRepository.GetAsync(@event.OrderId);
            orderToUpdate.SetCancelledStatus();
            await this._orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
