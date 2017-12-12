using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Locations.Api.Models;
using Locations.Api.ViewModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Bson;

namespace Locations.Api.Infrastructure.Respositorys
{
    public class LocationsRepository : ILocationsRepository
    {
        private readonly LocationsContext _context;

        public LocationsRepository(IOptions<LocationSettings> settings)
        {
            _context = new LocationsContext(settings);
        }

        public async Task AddUserLocationAsync(UserLocation location)
        {
            await _context.UserLocation.InsertOneAsync(location);
        }

        public async Task<Models.Locations> GetAsync(int locationId)
        {
            var filter = Builders<Models.Locations>.Filter.Eq("LocationId", locationId);
            return await this._context.Locations
                                      .Find(filter)
                                      .FirstOrDefaultAsync();
        }

        public async Task<List<Models.Locations>> GetCurrentUserRegionsListAsync(LocationRequest currentPosition)
        {
            var point = GeoJson.Point(GeoJson.Geographic(currentPosition.Longitude, currentPosition.Latitude));
            var orderByDistanceQuery = new FilterDefinitionBuilder<Models.Locations>().Near(x => x.Location, point);
            var withinAreaQuery = new FilterDefinitionBuilder<Models.Locations>().GeoIntersects("Polygon", point);
            var filter = Builders<Models.Locations>.Filter.And(orderByDistanceQuery, withinAreaQuery);
            return await _context.Locations.Find(filter).ToListAsync();
        }

        public async Task<List<Models.Locations>> GetLocationListAsync()
        {
            return await _context.Locations.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<UserLocation> GetUserLocationAsync(string userId)
        {
            var filter = Builders<UserLocation>.Filter.Eq("UserId", userId);
            return await _context.UserLocation
                                 .Find(filter)
                                 .FirstOrDefaultAsync();
        }

        public async Task UpdateUserLocationAsync(UserLocation userLocation)
        {
            await _context.UserLocation.ReplaceOneAsync(
                doc => doc.UserId == userLocation.UserId,
                userLocation,
                new UpdateOptions { IsUpsert = true });
        }
    }
}
