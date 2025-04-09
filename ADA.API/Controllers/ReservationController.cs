using ADA.API.IServices;
using ADA.API.Utility;
using ADAClassLibrary;
using ADAClassLibrary.DTOLibraries;
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



        //[HttpGet]
        //public async Task<ActionResult<string>> sendSMS(dynamic obj, bool sync, string Lang)
        //{
        //    var Gender = "";
        //    var GenderAR = "";
        //    var number = "971" + obj.PaxMobNum;
        //    var reservationMessage = "";
        //    var reservationMessageAR = "";

        //        Gender = obj.PaxGender == "M" ? "Mr" : "Ms/Mrs";

        //        GenderAR = obj.PaxGender == "M" ? "السيد" : "السيدة / آنسة";



        //    if (obj.RsvnStatus == "B")
        //    {
        //            reservationMessage = "Dear " + Gender + " " + obj.PaxName + ", your reservation is CONFIRMED for flight " + obj.FltNumber + " ETD: " + Convert.ToDateTime(obj.FltDateTime).ToString("dd-MMM-yyyy HH:mm") + ". Please come for check -in 2hr before the flight.";

        //            reservationMessageAR = "السيد/ة " + obj.PaxName + " تم تأكيد الحجز لرحلة رقم " + obj.FltNumber + " والمغادرة في الساعة [" + Convert.ToDateTime(obj.FltDateTime).ToString("dd-MMM-yyyy HH:mm") + "] . يرجى الحضور الى المطارقبل ساعة ونصف لعمل أجراءات السفر.";


        //        //reservationMessage = "Dear user, your flight "+obj.FltNumber+" has been CONFIRMED , Please call ADA Administration for any information.";
        //    }
        //    else if (obj.RsvnStatus == "W")
        //    {

        //            reservationMessage = "Dear " + Gender + " " + obj.PaxName + ", your reservation is WAITING for flight " + obj.FltNumber + " ETD: " + Convert.ToDateTime(obj.FltDateTime).ToString("dd-MMM-yyyy HH:mm") + ". Please come for check -in 2hr before the flight.";

        //        reservationMessageAR = "السيد/ة " + obj.PaxName + " تم الحجز في لأئحة الأنتظار لرحلة رقم " + obj.FltNumber + " والمغادرة في الساعة " + Convert.ToDateTime(obj.FltDateTime).ToString("dd-MMM-yyyy HH:mm") + ". يرجى الحضور الى المطارقبل ساعة ونصف لعمل أجراءات السفر.";


        //        //reservationMessage = "Dear user, your flight "+obj.FltNumber+" is in WAITING, Please call ADA Administration for any information.";

        //    }
        //    else if (obj.RsvnStatus == "D")
        //    {
        //            reservationMessage = "Dear " + Gender + " " + obj.PaxName + ", your reservation has been CANCELLED for flight " + obj.FltNumber + " ETD: " + Convert.ToDateTime(obj.FltDateTime).ToString("dd-MMM-yyyy HH:mm") + ". Please call ADA Administration for more information.";

        //            reservationMessageAR = "السيد/ة " + obj.PaxName + " تم ألغاء الحجز لرحلة رقم " + obj.FltNumber + " [والمغادرة في الساعة [" + Convert.ToDateTime(obj.FltDateTime).ToString("dd-MMM-yyyy HH:mm") + "] . يرجى الأتصال بأدارة طيران أبوظبي لمزيد من المعلومات.";


        //        //reservationMessage = "Dear user, your flight " + obj.FltNumber + " has been CANCELED , Please call ADA Administration for any information.";
        //    }

        //    List<string> urlobj = new List<string>();

        //    urlobj.Add("https://apiw.me.synapselive.com/push.aspx?user=AbuDhabiADAIT&pass=ad@adait&senderid=ADA&lang=0&mobile=" + number + "&lang=0&message=" + reservationMessage + "&dlr=1"); // sample url
        //    urlobj.Add("https://apiw.me.synapselive.com/push.aspx?user=AbuDhabiADAIT&pass=ad@adait&senderid=ADA&lang=8&mobile=" + number + "&lang=0&message=" + reservationMessageAR + "&dlr=1"); // sample url

        //    using (HttpClient client = new HttpClient())
        //    {
        //        foreach (var item in urlobj)
        //        {

        //            await client.GetStringAsync(item);

        //        }


        //    } 

        //    return null;
        //}


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


        [HttpPost("Add")]

        public Response Add([FromBody] Reservation obj)
        {
            //ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {

                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);

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
                    //response.Token = TokenManager.GenerateToken(claimDTO);
                    response.ResponseMsg = "Data save successfully!";
                }
                return response;



            }
            catch (DbException ex)
            {
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Add", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Add", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(600);
                //response.Token = TokenManager.GenerateToken(claimDTO);
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
                //response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = ex.Message;
                return response;
            }

        }

        [HttpPost("Update")]
        public Response Update(Reservation obj)
        {

            //ClaimDTO claimDTO = null;
            Response response = new Response();
            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);

                //var Permissions = JsonConvert.DeserializeObject<List<string>>(claimDTO.Permissions);
                ////   string Controller = ControllerContext.ActionDescriptor.ControllerName;
                //bool HasPermission = true;
                //if (!claimDTO.DesignationId.Contains(1))
                //{
                //    HasPermission = false;
                //    if (Permissions != null && Permissions.Count > 0 && Permissions.Contains(PermissionEnum.EditCompany.ToString()))
                //    {
                //        HasPermission = true;
                //    }
                //}
                //if (!HasPermission)
                //{
                //    response = CustomStatusResponse.GetResponse(403);
                //    response.Token = TokenManager.GenerateToken(claimDTO);
                //    return response;
                //}
                ////Get Data Logic Here
                //obj.ModifiedOn = DateTime.UtcNow.AddHours(UTCHours);
                //obj.CreatedBy = claimDTO.UserId;
                var res = _service.Update(obj);
                response = CustomStatusResponse.GetResponse(200);
                //response.Token = TokenManager.GenerateToken(claimDTO);
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
                //Get Data Login End
                return response;

            }
            catch (DbException ex)
            {
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Update", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Update", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

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
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Update", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Update", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(500);
                //response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = "Internal server error!";
                return response;
            }

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

                var res = _service.GetReservationsDropDown();
                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {
                    response.Data = res;
                    //response.Token = TokenManager.GenerateToken(claimDTO);
                }
                return response;



            }
            catch (DbException ex)
            {
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Add", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Add", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(600);
                //response.Token = TokenManager.GenerateToken(claimDTO);
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
                //response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = ex.Message;
                return response;
            }
        }



        [HttpPost("GetAll")]
        public Pagination GetAll()
        {
            ClaimDTO claimDTO = null;
            try
            {
                claimDTO = TokenManager.GetValidateToken(Request);

                if (claimDTO == null)
                {
                    Pagination response = new Pagination()
                    {
                        draw = "",
                        recordsFiltered = 0,
                        recordsTotal = 0,
                        Status = 401,
                        ResponseMsg = "unauthorized",
                        Token = null,
                        Data = null
                    };

                    return response;
                }
                // var Permissions = JsonConvert.DeserializeObject<List<string>>(claimDTO.Permissions);
                //string Controller = ControllerContext.ActionDescriptor.ControllerName;
                //bool HasPermission = true;
                //if (!claimDTO.DesignationId.Contains(1))
                //{
                //    HasPermission = false;
                //    if (Permissions != null && Permissions.Count > 0 && Permissions.Contains(PermissionEnum.ViewBranch.ToString()))
                //    {
                //        HasPermission = true;
                //    }
                //}
                // //if (!HasPermission)
                // //{
                // //    Pagination response = new Pagination()
                //     {
                //         draw = "",
                //         recordsFiltered = 0,
                //         recordsTotal = 0,
                //         Status = 403,
                //         ResponseMsg = "You don’t have permission to this action.",
                //       // Token = TokenManager.GenerateToken(claimDTO),
                //         Data = null
                //     };
                //     return response;
                //// }
                //HttpRequestMessage 
                //HttpRequestMessage m = new HttpRequestMessage();
                //var draw1 = HttpContext.Request.Form.Keys;

                var draw = HttpContext.Request.Headers["draw"].FirstOrDefault();
                var start = HttpContext.Request.Headers["start"].FirstOrDefault();
                var length = HttpContext.Request.Headers["length"].FirstOrDefault();
                var sortColumn = HttpContext.Request.Headers["sortColumn"].FirstOrDefault();
                var sortColumnDir = HttpContext.Request.Headers["sortColumnDir"].FirstOrDefault();
                var searchValue = HttpContext.Request.Headers["searchValue"].FirstOrDefault();
                //var start = HttpContext.Request.Form["sortColumnDir"].FirstOrDefault();
                //var length = HttpContext.Request.Form["length"].FirstOrDefault();
                //var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                //var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                var Data = cacheManager.TryGetValue(cacheName).ToList();



                //Sorting  
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



                //Search  
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    searchValue = searchValue.ToString().ToLower();

                //    Data = Data.Where(m => m.FltID.ToString().Contains(searchValue)


                //     || (m.ETD == null ? false : (DateTime.Parse(m.ETD).ToString("MM/dd/yyy hh:mm tt")).ToLower().Contains(searchValue))
                //     || (m.FltTimeStamp == null ? false : (m.FltTimeStamp.Value.ToString("MM/dd/yyy hh:mm tt")).ToLower().Contains(searchValue))
                //    || (m.Destination == null ? false : m.Destination.ToString().ToLower().Contains(searchValue))
                //    || (m.FltNumber == null ? false : m.FltNumber.ToString().ToLower().Contains(searchValue))
                //    || (m.Destination2 == null ? false : m.Destination2.ToString().ToLower().Contains(searchValue))

                //    || (m.Status == null ? false :m.Status.ToString().ToLower().Contains(searchValue))
                //    || (m.Aircraft == null ? false : m.Aircraft.ToString().ToLower().Contains(searchValue))
                //   || (m.Color == null ? false : m.Color.ToString().ToLower().Contains(searchValue))
                //   || (m.Pilot1 == null? false : m.Pilot1.ToString().ToLower().Contains(searchValue))
                //   || (m.Pilot2 == null ? false : m.Pilot2.ToString().ToLower().Contains(searchValue))
                //   || (m.Pilot3 == null ? false : m.Pilot3.ToString().ToLower().Contains(searchValue))
                //   || (m.FA1 == null ? false : m.FA1.ToString().ToLower().Contains(searchValue))
                //   || (m.FA2 == null? false : m.FA2.ToString().ToLower().Contains(searchValue))
                //   || (m.FA3 == null ? false : m.FA3.ToString().ToLower().Contains(searchValue))
                //   || (m.FA4 == null? false : m.FA4.ToString().ToLower().Contains(searchValue))
                //   || (m.Agent == null ? false : m.Agent.ToString().ToLower().Contains(searchValue))
                //   || (m.FltRemarks == null ? false : m.FltRemarks.ToString().ToLower().Contains(searchValue))
                //   || (m.SubManifestColor == null ? false : m.SubManifestColor.ToString().ToLower().Contains(searchValue))
                //   || (m.CustCode == null ? false : m.CustCode.ToString().ToLower().Contains(searchValue))
                //   || (m.FltRoute == null ? false : m.FltRoute.ToString().ToLower().Contains(searchValue))
                //   || (m.Payload.ToString().ToLower().Contains(searchValue))
                //   || (m.Fuel.ToString().Contains(searchValue))
                //   || (m.Temperature.ToString().ToLower().Contains(searchValue))
                //   || (m.GateNum.ToString().ToLower().Contains(searchValue))
                //   || (m.RsrvdSeats.ToString().ToLower().Contains(searchValue))
                //   //|| (m.UsePaxList.ToString().ToLower().Contains(searchValue == "1" ? "true" : "false"))
                //   //|| (m.SeatMap.ToString().ToLower().Contains(searchValue == "1" ? "true" : "false"))
                //   //|| (m.SplitGender.ToString().ToLower().Contains(searchValue == "1" ? "true" : "false"))
                //   //|| (m.ShowRCS.ToString().ToLower().Contains(searchValue == "1" ? "true" : "false"))
                //   || (m.FwdCargo1.ToString().ToLower().Contains(searchValue))
                //   || (m.FwdCargo2.ToString().ToLower().Contains(searchValue))
                //   || (m.FwdCargo3.ToString().ToLower().Contains(searchValue))
                //   || (m.FwdCargo4.ToString().ToLower().Contains(searchValue))
                //   || (m.AftCargo1.ToString().ToLower().Contains(searchValue))
                //   || (m.AftCargo2.ToString().ToLower().Contains(searchValue))
                //   || (m.AftCargo3.ToString().ToLower().Contains(searchValue))
                //   || (m.AftCargo4.ToString().ToLower().Contains(searchValue))
                //   || (m.AftCargo5.ToString().ToLower().Contains(searchValue))
                //   || (m.AftCargo6.ToString().ToLower().Contains(searchValue))
                //   || (m.ActualDepTime.ToString().ToLower().Contains(searchValue))


                //   )?.ToList();
                //}

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
                    Token = TokenManager.GenerateToken(claimDTO),
                    Data = Data
                };
                return pagination;
            }
            catch (DbException ex)
            {
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetAll", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetAll", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                Pagination pagination = new Pagination()
                {

                    //ResponseMsg = IsDBExceptionEnabeled ? "An Error Occured" : ex.Message,
                    Status = 600,
                    Token = TokenManager.GenerateToken(claimDTO),
                    Data = null,
                };
                return pagination;
            }
            catch (Exception ex)
            {
                //    WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetAll", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //    _loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetAll", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.RoleId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                Pagination pagination = new Pagination()
                {
                    ResponseMsg = "Internal server error!",
                    Status = 500,
                    Token = TokenManager.GenerateToken(claimDTO),
                    Data = null,
                };
                return pagination;
            }

        }


        [HttpPost("GetReservationBtID/{Id}")]
        public Response GetReservationBtID(int Id)
        {

            ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                claimDTO = TokenManager.GetValidateToken(Request);
                if (claimDTO == null) return CustomStatusResponse.GetResponse(401);



                var res = cacheManager.TryGetValue(cacheName).ToList().FirstOrDefault(x => x.RsvnID == Id);

                response = CustomStatusResponse.GetResponse(200);
                response.Token = TokenManager.GenerateToken(claimDTO);
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



        [HttpPost("GetMyBookingDetailsByID/{Id}")]
        public Response GetMyBookingDetailsByID(int Id)
        {

            ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);



                var res = _service.GetMyBookingDetailsByID(Id);

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

        [HttpPost("GetCancelBookingDetailsByUserId/{Id}")]
        public Response GetCancelBookingDetailsByUserId(int Id)
        {

            //ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);



                var res = _service.GetCancelBookingDetailsByUserId(Id);

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

        [HttpPost("CancelBookingByRsvnId")]
        public Response CancelBookingByRsvnId(Reservation obj)
        {

            //ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);



                var res = _service.CancelBookingByRsvnId(obj);

                response = CustomStatusResponse.GetResponse(200);
                //response.Token = TokenManager.GenerateToken(claimDTO);

                if (res != null)
                {

                    sendSMS(res[0], sync: true, obj.Lang).GetAwaiter().GetResult();
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
        
        [HttpPost("ReBookingByRsvnId")]
        public Response ReBookingByRsvnId(Reservation obj)
        {

            //ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);



                var res = _service.ReBookingByRsvnId(obj);

                response = CustomStatusResponse.GetResponse(200);
                //response.Token = TokenManager.GenerateToken(claimDTO);

                if (res != null)
                {

                    sendSMS(res[0], sync: true, obj.Lang).GetAwaiter().GetResult();
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

        [HttpPost("GetBordingPassDetails/{PNR}")]
        public Response GetBordingPassDetails(string PNR)
        {

            //ClaimDTO claimDTO = null;

            Response response = new Response();

            try
            {

                var res = _service.GetBordingPassDetails(PNR);

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
