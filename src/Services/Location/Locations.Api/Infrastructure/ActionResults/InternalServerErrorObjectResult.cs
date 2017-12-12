using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Locations.Api.Infrastructure.ActionResults
{
    public class InternalServerErrorObjectResult: ObjectResult
    {
        public InternalServerErrorObjectResult(object error) 
            : base(error)
        {
            this.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
