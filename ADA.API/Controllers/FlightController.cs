using ADA.API.Utility;
using ADAClassLibrary;
using ADAClassLibrary.DTOLibraries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthorizeAttribute = ADA.API.Utility.AuthorizeAttribute;
namespace ADA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IDIUnit services;
        private readonly IConfiguration _configuration;
        private readonly CacheManager<Flight> cacheManager;
        private readonly string cacheName = "Flight";
        private readonly IWebHostEnvironment _env;
        public FlightController(IWebHostEnvironment env, IDIUnit unit, IConfiguration confgiuration)
        {
            _configuration = confgiuration;
            services = unit;
            cacheManager = new CacheManager<Flight>(unit.memoryCache, unit.flightService);
            _env = env;

        }


        [AllowAnonymous]
        [HttpPost("GetAllDropdowns")]
        public Response GetAllDropDowns()
        {   
            Response response = new Response();
            try
            {
               
                var res = services.flightService.GetDropdownValues();
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
                response.ResponseMsg = ex.Message;
                return response;
            }
        }


        [Authorize]
        [HttpPost("GetFlightAndMembersDetails")]
        public Response GetFlightAndMembersDetails(GetFlightAndMembersDetails obj)
        {
            Response response = new Response();

            try
            {
                var MembersInformation = services.flightService.GetAllMembersDetails(obj.FltId,obj.MembersIds,obj.RegisterType);


                response = CustomStatusResponse.GetResponse(200);
                if (MembersInformation != null)
                {
                    response.Data = MembersInformation;

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


        [Authorize]
        [HttpPost("SearchFlight")]
        public Response SearchFlight( [FromBody] SearchFlight obj)
        {
            Response response = new Response();

            try
            {
    
                var res = services.flightService.GetSearchFlight(obj);

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

        [Authorize]
        [HttpPost("GetFlightDetailsAndNationality")]
        public Response GetFlightDetailsAndNationality(DestinationAndNationality obj)
        {

            Response response = new Response();

            try
            {

                var res = services.flightService.GetFlightDesitnationAndNationality(obj);

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

        [Authorize]
        [HttpPost("GetFlightStatusByFlightId/{FNo}")]
        public Response GetFlightStatusByFlightId(string FNo)
        {
            Response response = new Response();

            try
            {
                var res = services.flightService.GetFlightStatusByFlightId(FNo);

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


    


