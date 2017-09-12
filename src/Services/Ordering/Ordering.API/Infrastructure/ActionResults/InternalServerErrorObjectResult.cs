using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Infrastructure.ActionResults
{
    //
    // 摘要:
    //     An Microsoft.AspNetCore.Mvc.ObjectResult that when executed will produce a InternalServerError(500) response.
    public class InternalServerErrorObjectResult : ObjectResult
    {
        //
        // 摘要:
        //     Creates a new Ordering.API.Infrastructure.ActionResults.InternalServerErrorObjectResult instance.
        //
        // 参数:
        //   error:
        //     Contains the errors to be returned to the client.
        public InternalServerErrorObjectResult(object error) : base(error)
        {
            this.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
