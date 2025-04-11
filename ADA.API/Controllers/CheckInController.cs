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
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICheckInService _service;
        private readonly IWebHostEnvironment _env;
        private readonly double UTCHours = 5.0;
        private readonly string _controllerName = "CheckInController";
        private readonly string cacheName = "CheckIn";
        private readonly IConfiguration _configuration;
        private readonly CacheManager<CheckIn> cacheManager;
        public CheckInController(IWebHostEnvironment env, IMemoryCache memoryCache, ICheckInService service, IConfiguration confgiuration)
        {
            _env = env;
            _configuration = confgiuration;

            _memoryCache = memoryCache;

            _service = service;

            cacheManager = new CacheManager<CheckIn>(_memoryCache, service);
        }

        [Authorize]
        [HttpPost("Add")]
        public Response Add([FromBody] CheckIn obj)
        {
            ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                var cacheData = cacheManager.TryGetValue(cacheName).ToList();
                var res = _service.Add(obj);
                response = CustomStatusResponse.GetResponse(200);
             
                if (res != null)
                {

                    #region Set New Entry In Cache
                    cacheData.Add(res);
                    cacheManager.Remove(cacheName);
                    cacheManager.CreateEntry(cacheName, cacheData);

                    #endregion

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
        public Response Update(CheckIn obj)
        {
            Response response = new Response();
            try
            {
                var res = _service.Update(obj);
                response = CustomStatusResponse.GetResponse(200);
                if (res != null)
                {
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


                        Data = Data.OrderBy(item => typeof(CheckIn).GetProperty(sortColumn)?.GetValue(item)).ToList();
                    }
                    else
                    {
                        Data = Data.OrderByDescending(item => typeof(CheckIn).GetProperty(sortColumn)?.GetValue(item)).ToList();
                    }
                }


                recordsTotal = Data.Count();
          
                Data = Data.Skip(skip).Take(pageSize).ToList();

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
        [HttpPost("GetAllReserVationByFlightId/{Id}")]
        public Pagination GetAllReserVationByFlightId(int Id)
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


                CheckInDTO listobj = new CheckInDTO();


                var Data = _service.GetAllReserVationByFlightId(Id);



                var ResultData = listobj.CheckIn = Data.CheckIn.ToList();

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {


                    string str = sortColumn;


                    sortColumn = sortColumn = char.ToUpper(str[0]) + str.Substring(1);


                    if (sortColumnDir == "asc")
                    {


                        ResultData = ResultData.OrderBy(item => typeof(CheckIn).GetProperty(sortColumn)?.GetValue(item)).ToList();
                    }
                    else
                    {
                        ResultData = ResultData.OrderByDescending(item => typeof(CheckIn).GetProperty(sortColumn)?.GetValue(item)).ToList();
                    }
                }


                recordsTotal = ResultData.Count();
 
                if (!string.IsNullOrEmpty(searchValue))
                {
                    searchValue = searchValue?.ToLower();

                    ResultData = ResultData.Where(m =>
                     (m.GroupName.ToString().ToLower().Contains(searchValue))
                    || (m.RsvnPNR.ToString().ToLower().Contains(searchValue))
                    || (m.AdultPNR.ToString().ToLower().Contains(searchValue))
                    || (m.PaxName.ToString().ToLower().Contains(searchValue))
                    || (m.PaxIDNum.ToString().ToLower().Contains(searchValue))
                    || (m.PaxIDType.ToString().ToLower().Contains(searchValue))
                    || (m.PaxMobNum == null ? false : m.PaxMobNum.ToString().ToLower().Contains(searchValue))
                    || (m.PaxNationality.ToString().ToLower().Contains(searchValue))
                    || (m.PaxGender.ToString().ToLower().Contains(searchValue))
                    || (m.PaxCompany == null ? false : m.PaxCompany.ToString().ToLower().Contains(searchValue))
                    || (m.CardNum == null ? false : m.CardNum.ToString().ToLower().Contains(searchValue))
                    || (m.SeatNum == null ? false : m.SeatNum.ToString().ToLower().Contains(searchValue))
                    || (m.PaxWT.ToString().ToLower().Contains(searchValue))
                    || (m.BagWt.ToString().ToLower().Contains(searchValue))
                    || (m.BagPcs.ToString().ToLower().Contains(searchValue))
                    || (m.RsvEMail.ToString().ToLower().Contains(searchValue))
                    || (m.PaxBoarded.ToString().ToLower().Contains(searchValue))
                    || (m.PaxTransitDest.ToString().ToLower().Contains(searchValue))
                    || (m.RsvnStatus.ToString().ToLower().Contains(searchValue))
                    || (m.SMSTimeStamp.ToString().ToLower().Contains(searchValue))
                    || (m.RsvnChkTimeStamp.ToString().ToLower().Contains(searchValue))
                    || (m.RsvRmks == null ? false : m.RsvRmks.ToString().ToLower().Contains(searchValue))
                    || (m.RsvnChkAgent.ToString().ToLower().Contains(searchValue))
                    || (m.RsvnAgent.ToString().ToLower().Contains(searchValue))


                    )?.ToList();
                }

                if (sortColumnDir == "desc")
                {
                    ResultData = ResultData.Skip(skip).OrderByDescending(d => d.RsvnID).Take(pageSize).ToList();
                }
                else
                {
                    ResultData = ResultData.Skip(skip).Take(pageSize).ToList();
                }
                Pagination pagination = new Pagination()
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    Status = 200,
            
                    Data = new
                    {
                        dataobj = ResultData,

                        Aircraft = Data.Aircraft.ToList()
                    }
                };
                return pagination;
            }
            catch (DbException ex)
            {
                Pagination pagination = new Pagination()
                {

                    ResponseMsg = ex.Message,
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
        [HttpPost("GetuserDatabyRsvnId/{Id}")]
        public Response GetuserDatabyRsvnId(int Id)
        {
            Response response = new Response();

            try
            {
             
                var res = _service.GetuserDatabyRsvnId(Id);

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
        [HttpPost("GetCheckInByID/{Id}")]
        public Response GetCheckInByID(int Id)
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
        [HttpPost("GetMobileCheckin/{Id}")]
        public Response GetMobileCheckin(int Id)
        {
            Response response = new Response();

            try
            {
                var res = _service.GetFlightNoByUserId(Id);


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
        [HttpPost("GetMobileuserDatabyRsvnId/{Id}")]
        public Response GetMobileuserDatabyRsvnId(int Id)
        {


            Response response = new Response();

            try
            {
                var res = _service.GetuserDatabyRsvnId(Id);

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
        [HttpPost("ReservedSeatByCustomerInMobile")]
        public Response ReservedSeatByCustomerInMobile(CheckIn obj)
        {
            Response response = new Response();
            try
            {
                var res = _service.Update(obj);

                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {
                    response.Data = res;

                    response.ResponseMsg = "Seat Reserved successfully!";
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
        [HttpPost("GetCheckinCardDetails/{Id}")]
        public Response GetGetCheckinCardDetails(int Id)
        {
            Response response = new Response();

            try
            {
                var res = _service.GetCheckinCardDetails(Id);


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
