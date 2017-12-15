using Basket.API.Infrastructure.ActionResults;
using Basket.API.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Infrastructure.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this._env = env;
            this._logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            this._logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            if (context.Exception.GetType() == typeof(BasketDomainException))
            {
                var error = new JosnErrorResponse()
                {
                    Messages = new[] { context.Exception.Message }
                };

                context.Result = new BadRequestObjectResult(error);
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                var error = new JosnErrorResponse()
                {
                    Messages = new[] { context.Exception.Message }
                };

                if (this._env.IsDevelopment())
                {
                    error.DeveloperMessage = context.Exception;
                }

                context.Result = new InnerServerErrorObjectResult(error);
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            context.ExceptionHandled = true;
        }
    }
}
