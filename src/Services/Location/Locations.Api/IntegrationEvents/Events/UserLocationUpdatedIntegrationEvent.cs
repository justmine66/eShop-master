using EventBus.Events;
using Locations.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Locations.Api.IntegrationEvents.Events
{
    public class UserLocationUpdatedIntegrationEvent: IntegrationEvent
    {
        public string UserId { get; private set; }
        public List<UserLocationDetails> LocationList { get; private set; }

        public UserLocationUpdatedIntegrationEvent(string userId, List<UserLocationDetails> locationList)
        {
            this.UserId = userId;
            this.LocationList = locationList;
        }
    }
}
