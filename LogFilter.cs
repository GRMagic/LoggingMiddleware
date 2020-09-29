using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoggingMiddleware
{
    public class LoggedAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var verb = context.HttpContext.Request.Method;
            var url = UriHelper.GetDisplayUrl(context.HttpContext.Request);
            var route = context.ActionDescriptor.AttributeRouteInfo.Template;
            var fromRoute = context.HttpContext.Request.RouteValues?["anything"];
            var fromQuery = context.HttpContext.Request.Query?["another"].ToString();
            var status = context.HttpContext.Response.StatusCode;
            var response = (context.Result as Microsoft.AspNetCore.Mvc.ObjectResult)?.Value;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
