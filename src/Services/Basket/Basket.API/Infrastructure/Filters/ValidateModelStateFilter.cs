using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Basket.API.Infrastructure.Filters
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            var validatedErrors = context.ModelState
                .Keys
                .SelectMany(key => context.ModelState[key].Errors)
                .Select(m => m.ErrorMessage);

            var error = new JosnErrorResponse()
            {
                Messages = validatedErrors.ToArray()
            };

            context.Result = new BadRequestObjectResult(error);
        }
    }
}
