using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Abstractions;
using Locations.Api.Infrastructure.Exceptions;
using Locations.Api.Infrastructure.Respositorys;
using Locations.Api.IntegrationEvents.Events;
using Locations.Api.Models;
using Locations.Api.ViewModels;

namespace Locations.Api.Infrastructure.Services
{
    public class LocationsService : ILocationsService
    {
        private readonly ILocationsRepository _locationsRepository;
        private readonly IEventBus _eventBus;

        public LocationsService(ILocationsRepository locationsRepository, IEventBus eventBus)
        {
            this._locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
            this._eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task<bool> AddOrUpdateUserLocation(string userId, LocationRequest currentPosition)
        {
            // Get the list of ordered regions the user currently is within
            var currentUserAreaLocationList = await _locationsRepository.GetCurrentUserRegionsListAsync(currentPosition);

            if (currentUserAreaLocationList is null)
            {
                throw new LocationDomainException("User current area not found");
            }

            // If current area found, then update user location
            var locationAncestors = new List<string>();
            var userLocation = await _locationsRepository.GetUserLocationAsync(userId);
            userLocation = userLocation ?? new UserLocation();
            userLocation.UserId = userId;
            userLocation.LocationId = currentUserAreaLocationList[0].LocationId;
            userLocation.UpdateDate = DateTime.UtcNow;
            await _locationsRepository.UpdateUserLocationAsync(userLocation);

            // Publish integration event to update marketing read data model
            // with the new locations updated
            PublishNewUserLocationPositionIntegrationEvent(userId, currentUserAreaLocationList);

            return true;
        }

        public Task<List<Models.Locations>> GetAllLocation()
        {
            throw new NotImplementedException();
        }

        public async Task<Models.Locations> GetLocation(int locationId)
        {
            return await this._locationsRepository.GetAsync(locationId);
        }

        public async Task<UserLocation> GetUserLocation(string id)
        {
            return await _locationsRepository.GetUserLocationAsync(id);
        }

        private void PublishNewUserLocationPositionIntegrationEvent(string userId, List<Models.Locations> newLocations)
        {
            var newUserLocations = MapUserLocationDetails(newLocations);
            var @event = new UserLocationUpdatedIntegrationEvent(userId, newUserLocations);
            _eventBus.Publish(@event);
        }

        private List<UserLocationDetails> MapUserLocationDetails(List<Models.Locations> newLocations)
        {
            var result = new List<UserLocationDetails>();
            newLocations.ForEach(location => {
                result.Add(new UserLocationDetails()
                {
                    LocationId = location.LocationId,
                    Code = location.Code,
                    Description = location.Description
                });
            });

            return result;
        }
    }
}
