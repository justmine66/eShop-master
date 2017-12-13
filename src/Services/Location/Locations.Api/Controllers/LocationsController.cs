using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Locations.Api.Infrastructure.Services;
using Locations.Api.Models;
using Microsoft.AspNetCore.Http;
using System.Net;
using Locations.Api.ViewModels;

namespace Locations.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class LocationsController : Controller
    {
        private readonly ILocationsService _locationsService;
        private readonly IIdentityService _identityService;

        public LocationsController(ILocationsService locationsService, IIdentityService identityService)
        {
            this._locationsService = locationsService ?? throw new ArgumentNullException(nameof(locationsService));
            this._identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        //GET api/v1/Locations/user/1
        [Route("user/{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(UserLocation), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserLocation(Guid userId)
        {
            UserLocation userLocation = await this._locationsService.GetUserLocation(userId.ToString());
            return Ok(userLocation);
        }

        //GET api/v1/Locations
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(List<Models.Locations>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLocations()
        {
            List<Models.Locations> locations = await this._locationsService.GetAllLocation();
            return Ok(locations);
        }

        //GET api/v1/Locations/1
        [Route("{locationId:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(Models.Locations), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLocation(int locationId)
        {
            Models.Locations location = await this._locationsService.GetLocation(locationId);
            return Ok(location);
        }

        //GET api/v1/Locations/
        [Route("")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrUpdateUserLocation([FromBody]LocationRequest newLocReq)
        {
            string userId = this._identityService.GetUserIdentity();
            bool result = await this._locationsService.AddOrUpdateUserLocation(userId, newLocReq);

            return result ?
                Ok() as IActionResult :
                BadRequest() as IActionResult;
        }
    }
}