using ADAClassLibrary;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ADA.API.Utility
{
    public class AuthorizeUserIdDocumentAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _routeParameter;

        public AuthorizeUserIdDocumentAttribute(string routeParameter = "Id")
        {
            _routeParameter = routeParameter;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
          

            var userId = int.Parse(context.HttpContext.User.FindFirst("Id")?.Value);

            if (!context.RouteData.Values.TryGetValue(_routeParameter, out var routeValue) ||
                !int.TryParse(routeValue?.ToString(), out var routeId))
            {
                context.Result = new JsonResult(CustomStatusResponse.GetResponse(401));
                return;
            }

            if (userId != routeId)
            {
                context.Result = new JsonResult(CustomStatusResponse.GetResponse(401));
            }
        }
    }
}
