using ADA.API.IServices;
using ADA.API.Utility;
using ADAClassLibrary;
using ADAClassLibrary.DTOLibraries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AuthorizeAttribute = ADA.API.Utility.AuthorizeAttribute;

namespace ADA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class ReservationController : ControllerBase
    {
 
        private readonly IMemoryCache _memoryCache;
        private readonly IReservationService _service;
        private readonly IWebHostEnvironment _env;
        private readonly double UTCHours = 5.0;
        private readonly string _controllerName = "ReservationController";
        private readonly string cacheName = "Reservation";
        private readonly IConfiguration _configuration;
        private readonly CacheManager<Reservation> cacheManager;
        public ReservationController(IWebHostEnvironment env, IMemoryCache memoryCache, IReservationService service, IConfiguration confgiuration)
        {
            _env = env;
            _configuration = confgiuration;

            _memoryCache = memoryCache;

            _service = service;

            cacheManager = new CacheManager<Reservation>(_memoryCache, service);


        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<string>> sendSMS(dynamic obj, bool sync, string Lang)
        {
            var Gender = "";
            var GenderAR = "";
            var number = "971" + obj.PaxMobNum;
            var reservationMessage = "";
            var reservationMessageAR = "";

            Gender = obj.PaxGender == "M" ? "Mr" : "Ms/Mrs";
            GenderAR = obj.PaxGender == "M" ? "السيد" : "السيدة / آنسة";

            IConfiguration configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build();

            // Use the templates from appsettings.json to generate the reservation messages
            if (obj.RsvnStatus == "B")
            {
                reservationMessage = configuration["SMSMessageTemplates:ConfirmedReservationMessage"]
                    .Replace("{Gender}", Gender)
                    .Replace("{Name}", obj.PaxName)
                    .Replace("{FlightNumber}", obj.FltNumber)
                    .Replace("{FlightDateTime}", Convert.ToDateTime(obj.FltDateTime).ToString("dd-MMM-yyyy HH:mm"));

                reservationMessageAR = configuration["SMSMessageTemplates:ConfirmedReservationMessageAR"]
                    .Replace("{Name}", obj.PaxName)
                    .Replace("{FlightNumber}", obj.FltNumber)
                    .Replace("{FlightDateTime}", Convert.ToDateTime(obj.FltDateTime).ToString("dd-MMM-yyyy HH:mm"));

            } 
            
            if (obj.RsvnStatus == "W")
            {
                reservationMessage = configuration["SMSMessageTemplates:WaitingReservationMessage"]
                    .Replace("{Gender}", Gender)
                    .Replace("{Name}", obj.PaxName)
                    .Replace("{FlightNumber}", obj.FltNumber)
                    .Replace("{FlightDateTime}", Convert.ToDateTime(obj.FltDateTime).ToString("dd-MMM-yyyy HH:mm"));

                reservationMessageAR = configuration["SMSMessageTemplates:CancelledReservationMessageAR"]
                    .Replace("{Name}", obj.PaxName)
                    .Replace("{FlightNumber}", obj.FltNumber)
                    .Replace("{FlightDateTime}", Convert.ToDateTime(obj.FltDateTime).ToString("dd-MMM-yyyy HH:mm"));

            }
            
            if (obj.RsvnStatus == "D")
            {
                reservationMessage = configuration["SMSMessageTemplates:CancelledReservationMessage"]
                    .Replace("{Gender}", Gender)
                    .Replace("{Name}", obj.PaxName)
                    .Replace("{FlightNumber}", obj.FltNumber)
                    .Replace("{FlightDateTime}", Convert.ToDateTime(obj.FltDateTime).ToString("dd-MMM-yyyy HH:mm"));

                reservationMessageAR = configuration["SMSMessageTemplates:CancelledReservationMessageAR"]
                    .Replace("{Name}", obj.PaxName)
                    .Replace("{FlightNumber}", obj.FltNumber)
                    .Replace("{FlightDateTime}", Convert.ToDateTime(obj.FltDateTime).ToString("dd-MMM-yyyy HH:mm"));

            }

            List<string> urlobj = new List<string>();

            urlobj.Add("https://apiw.me.synapselive.com/push.aspx?user=AbuDhabiADAIT&pass=ad@adait&senderid=ADA&lang=0&mobile=" + number + "&lang=0&message=" + reservationMessage + "&dlr=1"); // sample url
            urlobj.Add("https://apiw.me.synapselive.com/push.aspx?user=AbuDhabiADAIT&pass=ad@adait&senderid=ADA&lang=8&mobile=" + number + "&lang=0&message=" + reservationMessageAR + "&dlr=1"); // sample url

            using (HttpClient client = new HttpClient())
            {
                foreach (var item in urlobj)
                {

                    await client.GetStringAsync(item);

                }


            }

            return null;
        }

        [Authorize]
        [HttpPost("Add")]

        public Response Add([FromBody] Reservation obj)
        {
            Response response = new Response();

            try
            {
                var res = _service.Add(obj);

                if (res!=null)
                {
                    for (int i = 0; i < res.Count(); i++)

                    {
                        sendSMS(res[i], sync: true, obj.Lang).GetAwaiter().GetResult();

                    }
                }
                response = CustomStatusResponse.GetResponse(200);
               
                if (res != null)
                {

                    response.Data = res;
                    response.ResponseMsg = "Data save successfully!";
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
        [HttpPost("Update")]
        public Response Update(Reservation obj)
        {
            Response response = new Response();
            try
            {
               
                var res = _service.Update(obj);
                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {
                    #region Set New Entry In Cache

                    var cacheData = cacheManager.TryGetValue(cacheName).ToList();
                    var oldObj = cacheData.FirstOrDefault(x => x.RsvnID == res.RsvnID);


                    cacheData.Remove(oldObj);
                    cacheData.Add(res);

                    cacheManager.Remove(cacheName);
                    cacheManager.CreateEntry(cacheName, cacheData);



                    #endregion

                    response.Data = res;
                    response.ResponseMsg = "Updated successfully!";
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

        [AllowAnonymous]
        [HttpPost("GetAllDropdowns")]
        public Response GetAllDropDowns()
        {
            Response response = new Response();
            try
            {

                var res = _service.GetReservationsDropDown();
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
        [HttpPost("GetAll")]
        public Pagination GetAll()
        {
            try
            {
                
                var draw = HttpContext.Request.Headers["draw"].FirstOrDefault();
                var start = HttpContext.Request.Headers["start"].FirstOrDefault();
                var length = HttpContext.Request.Headers["length"].FirstOrDefault();
                var sortColumn = HttpContext.Request.Headers["sortColumn"].FirstOrDefault();
                var sortColumnDir = HttpContext.Request.Headers["sortColumnDir"].FirstOrDefault();
                var searchValue = HttpContext.Request.Headers["searchValue"].FirstOrDefault();   
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = cacheManager.TryGetValue(cacheName).ToList();
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    string str = sortColumn;
                    sortColumn = sortColumn = char.ToUpper(str[0]) + str.Substring(1);
                    if (sortColumnDir == "asc")
                    {
                        Data = Data.OrderBy(item => typeof(Reservation).GetProperty(sortColumn)?.GetValue(item)).ToList();
                    }
                    else
                    {
                        Data = Data.OrderByDescending(item => typeof(Reservation).GetProperty(sortColumn)?.GetValue(item)).ToList();
                    }
                }

                recordsTotal = Data.Count();

                if (sortColumn == "Etd" && sortColumnDir == "desc")
                {
                    Data = Data.OrderByDescending(d => d.RsvnID).ToList();

                    Data = Data.Skip(skip).ToList();
                    Data = Data.Take(pageSize).ToList();


                }
                else
                {
                    Data = Data.Skip(skip).Take(pageSize).ToList();
                }
                Pagination pagination = new Pagination()
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    Status = 200,
                  
                    Data = Data
                };
                return pagination;
            }
            catch (DbException ex)
            {
              
                Pagination pagination = new Pagination()
                {
                    Status = 600,
                    Data = null,
                };
                return pagination;
            }
            catch (Exception ex)
            {
                Pagination pagination = new Pagination()
                {
                    ResponseMsg = "Internal server error!",
                    Status = 500,
                    Data = null,
                };
                return pagination;
            }

        }

        [Authorize]
        [HttpPost("GetReservationBtID/{Id}")]
        public Response GetReservationBtID(int Id)
        {
            Response response = new Response();

            try
            {
              
                var res = cacheManager.TryGetValue(cacheName).ToList().FirstOrDefault(x => x.RsvnID == Id);

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
        [HttpPost("GetMyBookingDetailsByID/{Id}")]
        public Response GetMyBookingDetailsByID(int Id)
        {

         
            Response response = new Response();

            try
            {
                var res = _service.GetMyBookingDetailsByID(Id);

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
        [HttpPost("GetCancelBookingDetailsByUserId/{Id}")]
        public Response GetCancelBookingDetailsByUserId(int Id)
        {
            Response response = new Response();

            try
            {
                var res = _service.GetCancelBookingDetailsByUserId(Id);

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
        [HttpPost("CancelBookingByRsvnId")]
        public Response CancelBookingByRsvnId(Reservation obj)
        {

          
            Response response = new Response();

            try
            {
                var res = _service.CancelBookingByRsvnId(obj);

                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {

                    sendSMS(res[0], sync: true, obj.Lang).GetAwaiter().GetResult();
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
        [HttpPost("ReBookingByRsvnId")]
        public Response ReBookingByRsvnId(Reservation obj)
        {
            Response response = new Response();

            try
            {
                var res = _service.ReBookingByRsvnId(obj);

                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {

                    sendSMS(res[0], sync: true, obj.Lang).GetAwaiter().GetResult();
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
        [HttpPost("GetBordingPassDetails/{PNR}")]
        public Response GetBordingPassDetails(string PNR)
        {

            Response response = new Response();

            try
            {

                var res = _service.GetBordingPassDetails(PNR);
                var userId = int.Parse(User.FindFirst("Id")?.Value);

                if (res.FirstOrDefault().UserId == userId)
                {


                    response = CustomStatusResponse.GetResponse(200);

                    if (res != null)
                    {

                        response.Data = res;


                    }
                }
                else
                {
                    response.Data = res;
                    response = CustomStatusResponse.GetResponse(401);
                    response.ResponseMsg = CustomStatusResponse.GetResponse(401).ResponseMsg;


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
