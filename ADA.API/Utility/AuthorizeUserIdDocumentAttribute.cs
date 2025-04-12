using ADAClassLibrary;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

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
            var httpContext = context.HttpContext;

            var token = httpContext.Request.Cookies["AuthToken"];

            if (token == null)
            {
                context.Result = new JsonResult(CustomStatusResponse.GetResponse(401));
                return;
            }

            var jsonToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                context.Result = new JsonResult(CustomStatusResponse.GetResponse(401));
                return;
            }

            var userIdClaim = jsonToken?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value;

            var userId = int.Parse(userIdClaim);

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
