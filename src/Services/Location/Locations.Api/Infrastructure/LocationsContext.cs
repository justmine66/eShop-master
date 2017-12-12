using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Locations.Api.Models;

namespace Locations.Api.Infrastructure
{
    public class LocationsContext
    {
        private readonly IMongoDatabase _database = null;

        public LocationsContext(IOptions<LocationSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
            {
                this._database = client.GetDatabase(settings.Value.Database);
            }
        }

        public IMongoCollection<UserLocation> UserLocation
        {
            get { return this._database.GetCollection<UserLocation>("UserLocation"); }
        }

        public IMongoCollection<Models.Locations> Locations
        {
            get
            {
                return _database.GetCollection<Models.Locations>("Locations");
            }
        }
    }
}
