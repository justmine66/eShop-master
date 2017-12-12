using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Locations.Api.Models;
using Locations.Api.ViewModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Locations.Api.Infrastructure.Respositorys
{
    public class LocationsRepository : ILocationsRepository
    {
        private readonly LocationsContext _context;

        public LocationsRepository(IOptions<LocationSettings> settings)
        {
            _context = new LocationsContext(settings);
        }

        public Task AddUserLocationAsync(UserLocation location)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.Locations> GetAsync(int locationId)
        {
            var filter = Builders<Models.Locations>.Filter.Eq("LocationId", locationId);
            return await this._context.Locations
                                      .Find(filter)
                                      .FirstOrDefaultAsync();
        }

        public Task<List<Models.Locations>> GetCurrentUserRegionsListAsync(LocationRequest currentPosition)
        {
            throw new NotImplementedException();
        }

        public Task<List<Models.Locations>> GetLocationListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserLocation> GetUserLocationAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserLocationAsync(UserLocation userLocation)
        {
            throw new NotImplementedException();
        }
    }
}
