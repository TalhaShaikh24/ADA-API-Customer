using ADA.IServices;
using ADAClassLibrary;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ADA.API.Utility
{
    public class AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            var token = httpContext.Request.Cookies["AuthToken"];

            if (token == null)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();

                return;
            }

            var jsonToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
               
                return;
            }

            var userIdClaim = jsonToken?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }

            var currentToken = await httpContext.RequestServices.GetRequiredService<IAuthenticationService>().GetSavedTokenAsync(int.Parse(userIdClaim));

            if (currentToken != token)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }

            var receviedtoken = httpContext.RequestServices.GetRequiredService<IAuthenticationService>().GetSavedTokenAsync(int.Parse(userIdClaim)).Result;

            if (string.IsNullOrEmpty(receviedtoken))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }

            var authService = httpContext.RequestServices.GetRequiredService<IAuthenticationService>();

            var isValid = await authService.IsTokenValidAsync(int.Parse(userIdClaim), receviedtoken);

            if (!isValid)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }
        }
    }
}
