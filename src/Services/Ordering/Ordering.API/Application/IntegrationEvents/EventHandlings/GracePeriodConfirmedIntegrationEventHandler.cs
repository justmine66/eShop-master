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
    /// 宽限期已确认集成事件处理
    /// </summary>
    public class GracePeriodConfirmedIntegrationEventHandler : IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public GracePeriodConfirmedIntegrationEventHandler(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }

        public async Task HandleAsync(GracePeriodConfirmedIntegrationEvent @event)
        {
            var orderToUpdate = await this._orderRepository.GetAsync(@event.OrderId);
            orderToUpdate.SetAwaitingValidationStatus();
            await this._orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
