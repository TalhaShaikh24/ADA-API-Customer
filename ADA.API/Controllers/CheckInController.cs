using ADA.API.IServices;
using ADA.API.Utility;
using ADAClassLibrary;
using ADAClassLibrary.DTOLibraries;
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


        [HttpPost("Add")]
        public Response Add([FromBody] CheckIn obj)
        {
            ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);
                // var Permissions = JsonConvert.DeserializeObject<List<string>>(claimDTO.Permissions);
                //  bool HasPermission = true;
                // if (!claimDTO.DesignationId.Contains(1))
                // {
                //  HasPermission = false;
                //  if (Permissions != null && Permissions.Count > 0 && Permissions.Contains(PermissionEnum.AddCompany.ToString()))
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
                var cacheData = cacheManager.TryGetValue(cacheName).ToList();
                //obj.ModifiedOn = DateTime.UtcNow.AddHours(UTCHours);
                //obj.CreatedBy = 0;//claimDTO.UserId;
                var res = _service.Add(obj);
                response = CustomStatusResponse.GetResponse(200);
                response.Token = TokenManager.GenerateToken(claimDTO);
                if (res != null)
                {

                    #region Set New Entry In Cache
                    //obj.Id = res;

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
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Add", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Add", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(600);
                response.Token = TokenManager.GenerateToken(claimDTO);
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
                //    WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Add", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //    _loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "Add", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(500);
                response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = ex.Message;
                return response;
            }

        }

        [HttpPost("Update")]
        public Response Update(CheckIn obj)
        {

            ClaimDTO claimDTO = null;
            Response response = new Response();
            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);

                var res = _service.Update(obj);
                response = CustomStatusResponse.GetResponse(200);
                //response.Token = TokenManager.GenerateToken(claimDTO);
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
                response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = ex.Message;
                return response;
            }
            catch (Exception ex)
            {
                response = CustomStatusResponse.GetResponse(500);
                response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = "Internal server error!";
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



                //if (!claimDTO.DesignationId.Contains(1))
                //{
                //   // Data = Data.Where(x => claimDTO.Branches.Contains(x.Id.Value)).ToList();
                //}
                //Data.RemoveAll(c => c.Id == 1 && c.Name == "All");

                //var companyCache = new CacheManager<Company>(_memoryCache, _companyService).TryGetValue(CompanycacheName).ToList();

                //for (int i = 0; i < Data.Count; i++)
                //{
                //    Data[i].CompanyName = companyCache.FirstOrDefault(x => x.Id == Data[i].CountryId).Name;


                //}
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {


                    string str = sortColumn;


                    sortColumn = sortColumn = char.ToUpper(str[0]) + str.Substring(1);

                    //CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    //TextInfo textInfo = cultureInfo.TextInfo;

                    //sortColumn= sortColumn=textInfo.ToTitleCase(sortColumn);

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
                //Search  
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    searchValue = searchValue?.ToLower();

                //    Data = Data.Where(m =>m.CheckInName.ToString().ToLower().Contains(searchValue)

                //    || (m.ModifiedOn == null ? false :m.ModifiedOn.Value.ToString("dd/MM/yyy hh:mm:ss tt").ToLower().Contains(searchValue))

                //    )?.ToList();
                //}



                //total number of rows count     

                //Paging  
                //List<Branch> list = new List<Branch>();
                //if (sortColumn == "ModifiedOn" && sortColumnDir == "desc")
                //{
                //    Data = Data.Skip(skip).OrderByDescending(d => d.ModifiedOn).Take(pageSize).ToList();
                //}
                //else
                //{
                Data = Data.Skip(skip).Take(pageSize).ToList();
                //}
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
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetAll", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetAll", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

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
                //    WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetAll", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //    _loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetAll", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

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

        [HttpPost("GetAllReserVationByFlightId/{Id}")]
        public Pagination GetAllReserVationByFlightId(int Id)
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


                var draw = HttpContext.Request.Headers["draw"].FirstOrDefault();
                var start = HttpContext.Request.Headers["start"].FirstOrDefault();
                var length = HttpContext.Request.Headers["length"].FirstOrDefault();
                var sortColumn = HttpContext.Request.Headers["sortColumn"].FirstOrDefault();
                var sortColumnDir = HttpContext.Request.Headers["sortColumnDir"].FirstOrDefault();
                var searchValue = HttpContext.Request.Headers["searchValue"].FirstOrDefault();

                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                CheckInDTO listobj = new CheckInDTO();


                var Data = _service.GetAllReserVationByFlightId(Id);



                var ResultData = listobj.CheckIn = Data.CheckIn.ToList();


                //Sorting  
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

                //Search  
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



                //total number of rows count     

                //Paging  
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
                    Token = TokenManager.GenerateToken(claimDTO),
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
                    Token = TokenManager.GenerateToken(claimDTO),
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
                    Token = TokenManager.GenerateToken(claimDTO),
                    Data = null,
                };
                return pagination;
            }

        }


        [HttpPost("GetuserDatabyRsvnId/{Id}")]
        public Response GetuserDatabyRsvnId(int Id)
        {

            ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                claimDTO = TokenManager.GetValidateToken(Request);
                if (claimDTO == null) return CustomStatusResponse.GetResponse(401);


                var res = _service.GetuserDatabyRsvnId(Id);

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
                response = CustomStatusResponse.GetResponse(600);
                response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = ex.Message;
                return response;
            }
            catch (Exception ex)
            {
                response = CustomStatusResponse.GetResponse(500);
                response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = "Internal server error!";
                return response;
            }

        }


        [HttpPost("GetCheckInByID/{Id}")]
        public Response GetCheckInByID(int Id)
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
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

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
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(500);
                response.Token = TokenManager.GenerateToken(claimDTO);
                response.ResponseMsg = "Internal server error!";
                return response;
            }

        }


        [HttpPost("GetMobileCheckin/{Id}")]
        public Response GetMobileCheckin(int Id)
        {

            //ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);

                var res = _service.GetFlightNoByUserId(Id);


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
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

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
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(500);

                 //response.Token = TokenManager.GenerateToken(claimDTO);

                response.ResponseMsg = "Internal server error!";
                return response;
            }

        }


        [HttpPost("GetMobileuserDatabyRsvnId/{Id}")]
        public Response GetMobileuserDatabyRsvnId(int Id)
        {

            //ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);


                var res = _service.GetuserDatabyRsvnId(Id);

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


        [HttpPost("ReservedSeatByCustomerInMobile")]
        public Response ReservedSeatByCustomerInMobile(CheckIn obj)
        {
            //ClaimDTO claimDTO = null;
            Response response = new Response();
            //claimDTO = TokenManager.GetValidateToken(Request);
            //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);
            try
            {
                var res = _service.Update(obj);

                response = CustomStatusResponse.GetResponse(200);
                //response.Token = TokenManager.GenerateToken(claimDTO);

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


        [HttpPost("GetCheckinCardDetails/{Id}")]
        public Response GetGetCheckinCardDetails(int Id)
        {

            //ClaimDTO claimDTO = null;
            Response response = new Response();

            try
            {
                //claimDTO = TokenManager.GetValidateToken(Request);
                //if (claimDTO == null) return CustomStatusResponse.GetResponse(401);



                var res = _service.GetCheckinCardDetails(Id);


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
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 600, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

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
                //WriteFileLogger.WriteLog(_env, Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));
                //_loggerService.CreateLog(Convert.ToString(Request.Path.HasValue == false ? "" : Request.Path.Value), _controllerName, "GetBranchById", claimDTO.Username, Convert.ToInt32(claimDTO.UserId), claimDTO.CheckInId, 500, Convert.ToString(ex.Message), Convert.ToString(ex.InnerException));

                response = CustomStatusResponse.GetResponse(500);

                //response.Token = TokenManager.GenerateToken(claimDTO);

                response.ResponseMsg = "Internal server error!";
                return response;
            }

        }


    }
}
