using ADA.API.IServices;
using ADA.API.Utility;
using ADAClassLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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
    public class CargoController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICargoService _service;
        private readonly IWebHostEnvironment _env;
        private readonly double UTCHours = 5.0;
        private readonly string _controllerName = "CargoController";
        private readonly string cacheName = "Cargo";
        private readonly IConfiguration _configuration;
        private readonly CacheManager<Cargo> cacheManager;
        public CargoController(IWebHostEnvironment env, IMemoryCache memoryCache, ICargoService service, IConfiguration confgiuration)
        {
            _env = env;
            _configuration = confgiuration;

            _memoryCache = memoryCache;

            _service = service;


            cacheManager = new CacheManager<Cargo>(_memoryCache, service);


        }
        [Authorize]
        [HttpPost("GetAllCargoStatusByBarcode/{Barcode}")]
        public Response GetAllCargoStatusByBarcode(string Barcode)
        {

            Response response = new Response();

            try
            {

                var res = _service.GetAllCargoStatusByBarcode(Barcode);

                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {


                    response.Data = res;


                }

                return response;



            }
            catch (DbException ex)
            {
                response = CustomStatusResponse.GetResponse(600);
                response.ResponseMsg = ex.Message;
                return response;
            }
            catch (Exception ex)
            {
                response = CustomStatusResponse.GetResponse(500);
                response.ResponseMsg = "Internal server error!";
                return response;
            }

        }
    }
}
