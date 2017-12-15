using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Infrastructure.ActionResults
{
    public class InnerServerErrorObjectResult : ObjectResult
    {
        public InnerServerErrorObjectResult(object error)
            : base(error)
        {
            this.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
