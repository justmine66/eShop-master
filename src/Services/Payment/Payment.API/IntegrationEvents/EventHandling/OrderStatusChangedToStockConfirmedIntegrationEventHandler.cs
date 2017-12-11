using EventBus.Abstractions;
using Payment.API.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EventBus.Events;

namespace Payment.API.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToStockConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly PaymentSettings _settings;

        public OrderStatusChangedToStockConfirmedIntegrationEventHandler(
            IEventBus eventBus,
            IOptionsMonitor<PaymentSettings> settings)
        {
            this._eventBus = eventBus;
            this._settings = settings.CurrentValue;
        }

        public async Task Handle(OrderStatusChangedToStockConfirmedIntegrationEvent @event)
        {
            IntegrationEvent orderPaymentIntegrationEvent;

            if (this._settings.PaymentSuccessed)
            {
                orderPaymentIntegrationEvent = new OrderPaymentSuccededIntegrationEvent(@event.OrderId);
            }
            else
            {
                orderPaymentIntegrationEvent = new OrderPaymentFailedIntegrationEvent(@event.OrderId);
            }

            this._eventBus.Publish(orderPaymentIntegrationEvent);

            await Task.CompletedTask;
        }
    }
}
