using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Locations.Api.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Locations.Api.Infrastructure.ActionResults;

namespace Locations.Api.Infrastructure.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(
            IHostingEnvironment environment,
            ILogger<HttpGlobalExceptionFilter> logger)
        {
            this._environment = environment;
            this._logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            this._logger.LogError(
                eventId: new EventId(context.Exception.HResult),
                exception: context.Exception,
                message: context.Exception.Message);

            if (context.GetType() == typeof(LocationDomainException))
            {
                var json = new JsonErrorResponse()
                {
                    Messages = new[] { context.Exception.Message }
                };

                context.Result = new BadRequestObjectResult(json);
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                var json = new JsonErrorResponse()
                {
                    Messages = new[] { Resources.Exception_Unknown }
                };

                if (this._environment.IsDevelopment())
                {
                    json.DeveloperMessage = context.Exception;
                };

                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            context.ExceptionHandled = true;
        }

        private class JsonErrorResponse
        {
            public string[] Messages { get; set; }
            public object DeveloperMessage { get; set; }
        }
    }
}
