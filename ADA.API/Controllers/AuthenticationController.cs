using ADA.API.Helpers;
using ADA.API.Utility;
using ADA.IServices;
using ADAClassLibrary;
using ADAClassLibrary.DTOLibraries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AuthorizeAttribute = ADA.API.Utility.AuthorizeAttribute;

namespace ADA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly TokenManager _tokenManager;
        private readonly IMemoryCache _memoryCache;
        private readonly IAuthenticationService _authenticationService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        public bool IsDBExceptionEnabeled = false;
        private readonly EncryptionService _encryptionService;

        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IConfiguration confgiuration, IAuthenticationService authenticationService, IMemoryCache memoryCache, IWebHostEnvironment env, TokenManager tokenManager, EncryptionService encryptionService, ILogger<AuthenticationController> logger)
        {
            _configuration = confgiuration;
            _memoryCache = memoryCache;
            _authenticationService = authenticationService;
            _env = env;
            _tokenManager = tokenManager;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<Response> Authenticate(LoginCredentials obj)
        {
            Response response = new Response();
            ClaimDTO claimDTO = new ClaimDTO();
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");



                obj.Password = _encryptionService.Encrypt(obj.Password);
                var user = _authenticationService.Authenticate(obj);
                if (user == null) return CustomStatusResponse.GetResponse(320);
                else
                {

                    var TokenExpiryDate = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryInMinutes"]));

                    response = CustomStatusResponse.GetResponse(200);
                    response.Token = _tokenManager.GenerateJwtToken(user);
                    response.Data = new
                    {
                        DataObj = user,
                    };

                    //Response.Cookies.Append("AuthToken", response.Token, new CookieOptions
                    //{
                    //    Path = "/",
                    //    HttpOnly = true,
                    //    Secure = false,
                    //    SameSite = SameSiteMode.Lax,
                    //    Expires = TokenExpiryDate
                    //});

                  //  _logger.LogInformation("AuthToken Saved." + HttpContext.Request.Cookies["AuthToken"]);


                    await _authenticationService.SaveUserToken(user.Id, response.Token, DateTime.UtcNow, TokenExpiryDate);
                    
                    return response;
                }
            }
            catch (DbException ex)
            {
                response = CustomStatusResponse.GetResponse(600);
                if (IsDBExceptionEnabeled)
                {
                    response.ResponseMsg = "An Error Occured";
                }
                else
                {
                    response.ResponseMsg = ex.Message;
                }
                return response;
            }
            catch (Exception ex)
            {
                response = CustomStatusResponse.GetResponse(500);
                response.ResponseMsg = ex.Message;
                return response;
            }
        }


        [Authorize]
        [HttpPost("Logout")]
        public Response Logout()
        {
            Response response;
            try
            {


                var userId = int.Parse(User.FindFirst("Id")?.Value);

                var token = Request.Headers["Authorization"].ToString();

                if (token != null)
                {
                    _authenticationService.LogoutAsync(userId,token);
                }

                response = CustomStatusResponse.GetResponse(200);
                response.Data = null;
                response.Token = null;
                return response;

            }
            catch (DbException ex)
            {
                response = CustomStatusResponse.GetResponse(600);
                response.Token = null;
                if (IsDBExceptionEnabeled)
                {
                    response.ResponseMsg = "An Error Occured";
                }
                else
                {

                    response.ResponseMsg = ex.Message;
                }
                return response;
            }
            catch (Exception ex)
            {
                response = CustomStatusResponse.GetResponse(500);
                response.Token = null;
                response.ResponseMsg = "Internal server error!";
                return response;
            }
        }





    }


}

