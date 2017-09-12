using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Ordering.API.Infrastructure.ActionResults;
using Ordering.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Ordering.API.Infrastructure.Filters
{
    /// <summary>
    /// Http 全局异常过滤器
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            logger.LogError(new EventId(context.Exception.HResult),
                context.Exception, context.Exception.Message);

            if (context.Exception.GetType() == typeof(OrderingDomainException))//可预知的异常
            {
                var json = new JsonErrorResponse
                {
                    Messages = new[] { context.Exception.Message }
                };

                context.Result = new BadRequestObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else//不可预知的异常
            {
                var json = new JsonErrorResponse
                {
                    Messages = new[] { "A error occur.Try it again." }
                };

                if (this.env.IsDevelopment())//开发环境
                {
                    json.DeveloperMessage = context.Exception;
                }

                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        private class JsonErrorResponse
        {
            /// <summary>
            /// 友好错误消息列表
            /// </summary>
            public string[] Messages { get; set; }

            /// <summary>
            /// 开发人员消息
            /// </summary>
            public object DeveloperMessage { get; set; }
        }
    }
}
