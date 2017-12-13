using Locations.Api.Models;
using Locations.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Locations.Api.Infrastructure.Services
{
    public interface ILocationsService
    {
        Task<Models.Locations> GetLocation(int locationId);
        Task<UserLocation> GetUserLocation(string id);
        Task<List<Models.Locations>> GetAllLocation();
        Task<bool> AddOrUpdateUserLocation(string userId, LocationRequest locRequest);
    }
}
