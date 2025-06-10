using ADA.IServices;
using ADAClassLibrary;
using Microsoft.AspNetCore.Mvc;
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
          

            var token = context.HttpContext.Request.Headers["Authorization"].ToString();

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

            if (string.IsNullOrEmpty(userIdClaim))
            {
                context.Result = new JsonResult(CustomStatusResponse.GetResponse(401));
                return;
            }

            var currentToken = await context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>().GetSavedTokenAsync(int.Parse(userIdClaim));

            if (currentToken != token)
            {
                context.Result = new JsonResult(CustomStatusResponse.GetResponse(401));
                return;
            }

            var receviedtoken = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>().GetSavedTokenAsync(int.Parse(userIdClaim)).Result;

            if (string.IsNullOrEmpty(receviedtoken))
            {
                context.Result = new JsonResult(CustomStatusResponse.GetResponse(401));
                return;
            }

            var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();

            var isValid = await authService.IsTokenValidAsync(int.Parse(userIdClaim), receviedtoken);

            if (!isValid)
            {
                context.Result = new JsonResult(CustomStatusResponse.GetResponse(401));
                return;
            }

            var identity = new ClaimsIdentity(jsonToken.Claims, "jwt");

            context.HttpContext.User = new ClaimsPrincipal(identity);

        }
    }
}
