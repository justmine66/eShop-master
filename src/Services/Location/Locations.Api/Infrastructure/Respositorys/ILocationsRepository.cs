using Locations.Api.Models;
using Locations.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Locations.Api.Infrastructure.Respositorys
{
    public interface ILocationsRepository
    {
        Task<Locations.Api.Models.Locations> GetAsync(int locationId);

        Task<List<Locations.Api.Models.Locations>> GetLocationListAsync();

        Task<UserLocation> GetUserLocationAsync(string userId);

        Task<List<Locations.Api.Models.Locations>> GetCurrentUserRegionsListAsync(LocationRequest currentPosition);

        Task AddUserLocationAsync(UserLocation location);

        Task UpdateUserLocationAsync(UserLocation userLocation);
    }
}
