using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Events;

namespace Basket.API.Aplication.IntegrationEvents.Events
{
    public class OrderStartedIntegrationEvent: IntegrationEvent
    {
        public string UserId { get; set; }

        public OrderStartedIntegrationEvent(string userId)
            => this.UserId = userId;
    }
}
