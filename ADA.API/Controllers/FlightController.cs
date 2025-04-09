using ADA.API.Utility;
using ADAClassLibrary;
using ADAClassLibrary.DTOLibraries;
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

namespace ADA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IDIUnit services;

        //public double ExpireTime;
        //private readonly string _controllerName = "AirCraftController";
        //private readonly ILoggerService _loggerService;
        private readonly IConfiguration _configuration;


        private readonly CacheManager<Flight> cacheManager;

        private readonly string cacheName = "Flight";

        //private readonly string cacheNameAircrafts = "Aircraft";
        //private readonly string cacheNameDestinations = "Destinatiion";
        //private readonly string cacheNamePilots = "Pilot";
        //private readonly string cacheNameStaff = "Staff";
        //private readonly string cacheNameFlightStatus = "FlightStatus";

        //private readonly string authenticationCacheName = "Authentication";
        //private readonly double UTCHours = 5.0;
        private readonly IWebHostEnvironment _env;
        public FlightController(IWebHostEnvironment env, IDIUnit unit, IConfiguration confgiuration)
        {
            _configuration = confgiuration;
            services = unit;
            cacheManager = new CacheManager<Flight>(unit.memoryCache, unit.flightService);

            // _loggerService = loggerService;
            _env = env;

        }



        [HttpPost("GetAllDropdowns")]
        public Response GetAllDropDowns()
        {   

            //ClaimDTO claimDTO = null;
            Response response = new Response();
            //claimDTO = TokenManager.GetValidateToken(Request);
            //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);
            try
            {
               
                var res = services.flightService.GetDropdownValues();
                response = CustomStatusResponse.GetResponse(200);
                //response.Token = TokenManager.GenerateToken(claimDTO);

                if (res != null)
                {
                    response.Data = res;
                   
                }
                return response;



            }
            catch (DbException ex)
            {
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Add", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Add", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(600);
               // response.Token = TokenManager.GenerateToken(claimDTO);
                //if (IsDBExceptionEnabeled)
                //{
                //    response.ResponseMsg = "An Error Occured";
                //}
                //else
                //{

                response.ResponseMsg = ex.Message;
                // }

                return response;
            }
            catch (Exception ex)
            {
                //    WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Add", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //    _loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Add", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(500);
               // response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = ex.Message;
                return response;
            }
        }


        [HttpPost("GetFlightAndMembersDetails")]
        public Response GetFlightAndMembersDetails(GetFlightAndMembersDetails obj)
        {

            //ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);

                //var res = cacheManager.TryGetValue(cacheName).ToList().FirstOrDefault(x => x.FltID == obj.FltId);


                var MembersInformation = services.flightService.GetAllMembersDetails(obj.FltId,obj.MembersIds,obj.RegisterType);


                response = CustomStatusResponse.GetResponse(200);
                //response.Token = TokenManager.GenerateToken(claimDTO);
                if (MembersInformation != null)
                {

                    //response.Token = TokenManager.GenerateToken(claimDTO);
                    response.Data = MembersInformation;


                }
               
                return response;



            }
            catch (DbException ex)
            {
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(600);
                //response.Token = TokenManager.GenerateToken(claimDTO);
                //if (IsDBExceptionEnabeled)
                //{
                //    response.ResponseMsg = "An Error Occured";
                //}
                //else
                //{

                response.ResponseMsg = ex.Message;
                //}

                return response;
            }
            catch (Exception ex)
            {
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(500);
               // response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = "Internal server error!";
                return response;
            }
        }






        [HttpPost("SearchFlight")]
        public Response SearchFlight( [FromBody] SearchFlight obj)
        {

            ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);



                var res = services.flightService.GetSearchFlight(obj);

                response = CustomStatusResponse.GetResponse(200);
               // response.Token = TokenManager.GenerateToken(claimDTO);
                if (res != null)
                {


                    response.Data = res;


                }

                return response;



            }
            catch (DbException ex)
            {
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(600);
                response.Token = TokenManager.GenerateToken(claimDTO);
                //if (IsDBExceptionEnabeled)
                //{
                //    response.ResponseMsg = "An Error Occured";
                //}
                //else
                //{

                response.ResponseMsg = ex.Message;
                //}

                return response;
            }
            catch (Exception ex)
            {
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(500);
                response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = "Internal server error!";
                return response;
            }

        }


        [HttpPost("GetFlightDetailsAndNationality")]
        public Response GetFlightDetailsAndNationality(DestinationAndNationality obj)
        {

            ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);

                var res = services.flightService.GetFlightDesitnationAndNationality(obj);

                response = CustomStatusResponse.GetResponse(200);
                //response.Token = TokenManager.GenerateToken(claimDTO);
                if (res != null)
                {


                    response.Data = res;


                }

                return response;



            }
            catch (DbException ex)
            {
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(600);
                //response.Token = TokenManager.GenerateToken(claimDTO);
                //if (IsDBExceptionEnabeled)
                //{
                //    response.ResponseMsg = "An Error Occured";
                //}
                //else
                //{

                response.ResponseMsg = ex.Message;
                //}

                return response;
            }
            catch (Exception ex)
            {
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(500);
                // response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = "Internal server error!";
                return response;
            }

        }


        [HttpPost("GetFlightStatusByFlightId/{FNo}")]
        public Response GetFlightStatusByFlightId(string FNo)
        {

            //ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);



                var res = services.flightService.GetFlightStatusByFlightId(FNo);

                response = CustomStatusResponse.GetResponse(200);
                //response.Token = TokenManager.GenerateToken(claimDTO);
                if (res != null)
                {


                    response.Data = res;


                }

                return response;



            }
            catch (DbException ex)
            {
                response = CustomStatusResponse.GetResponse(600);
                //response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = ex.Message;

                return response;
            }
            catch (Exception ex)
            {
                response = CustomStatusResponse.GetResponse(500);
                //response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = "Internal server error!";
                return response;
            }

        }

    }





}


    


