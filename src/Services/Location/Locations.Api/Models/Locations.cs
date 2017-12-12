﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Collections.Generic;

namespace Locations.Api.Models
{
    public class Locations
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int LocationId { get; set; }

        public string Code { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Parent_Id { get; set; }

        public string Description { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; private set; }
        public GeoJsonPolygon<GeoJson2DGeographicCoordinates> Polygon { get; private set; }

        public void SetLocation(double lon, double lat) => this.SetPosition(lon, lat);
        public void SetArea(List<GeoJson2DGeographicCoordinates> coordinatesList) => this.SetPolygon(coordinatesList);

        private void SetPosition(double lon,double lat)
        {
            this.Latitude = lat;
            this.Longitude = lon;
            this.Location = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(lon, lat));
        }
        private void SetPolygon(List<GeoJson2DGeographicCoordinates> coordinatesList)
        {
            this.Polygon = new GeoJsonPolygon<GeoJson2DGeographicCoordinates>(new GeoJsonPolygonCoordinates<GeoJson2DGeographicCoordinates>(
                 new GeoJsonLinearRingCoordinates<GeoJson2DGeographicCoordinates>(coordinatesList)));
        }
    }
}