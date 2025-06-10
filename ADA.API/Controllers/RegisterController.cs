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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using AuthorizeAttribute = ADA.API.Utility.AuthorizeAttribute;
using ADA.API.Helpers;
namespace ADA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IRegisterService _service;
        private readonly IWebHostEnvironment _env;
        private readonly double UTCHours = 5.0;
        private readonly string _controllerName = "RegisterController";
        private readonly string cacheName = "Register";
        private readonly IConfiguration _configuration;
        private readonly CacheManager<Register> cacheManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EncryptionService _encryptionService;
        public RegisterController(
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor, 
            IMemoryCache memoryCache, 
            IRegisterService service,
            EncryptionService encryptionService,
            IConfiguration confgiuration)
        {
            _env = env;
            _configuration = confgiuration;

            _memoryCache = memoryCache;
            _encryptionService = encryptionService;

            _service = service;

            cacheManager = new CacheManager<Register>(_memoryCache, service);

            _httpContextAccessor = httpContextAccessor;
        }

        [AllowAnonymous]
        [HttpPost("Add")]
        public Response Add([FromForm] Register obj)
        {
            Response response = new Response();
            try
            {
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var files = Request.Form.Files;
                    obj.FileName = new List<string>();

                    foreach (IFormFile photo in files)
                    {
                        var supportedTypes = new[] { "doc", "docx", "jpeg", "jpg", "png", "pdf" };
                        var fileExt = Path.GetExtension(photo.FileName).Substring(1).ToLower();

                        if (!supportedTypes.Contains(fileExt))
                        {
                            response = CustomStatusResponse.GetResponse(600);
                            response.ResponseMsg = "Invalid file extension.";
                            return response;
                        }

                        var GuidFileName = Guid.NewGuid().ToString("n") + photo.FileName;
                        var path = Path.Combine(this._env.WebRootPath, "Documents", GuidFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            photo.CopyTo(stream);
                        }

                        obj.FileName.Add("https://hajzapi.ada.ae/Documents/" + GuidFileName);
                    }
                }

                var encryptedPassword = _encryptionService.Encrypt(HttpContext.Request.Form["Password"].FirstOrDefault());

                obj.Honorifics = HttpContext.Request.Form["Honorifics"].FirstOrDefault();
                obj.Name = HttpContext.Request.Form["Name"].FirstOrDefault();
                obj.Email = string.IsNullOrEmpty(HttpContext.Request.Form["Email"].FirstOrDefault())
                        ? HttpContext.Request.Form["Mobile"].FirstOrDefault() + "@dummy.ada.ae"
                        : obj.Email;
                obj.Nationality = HttpContext.Request.Form["Nationality"].FirstOrDefault();
                obj.Username = HttpContext.Request.Form["Username"].FirstOrDefault();
                obj.Password = encryptedPassword;
                obj.Birthdate = Convert.ToDateTime(HttpContext.Request.Form["Birthdate"].FirstOrDefault());
                obj.Mobile = HttpContext.Request.Form["Mobile"].FirstOrDefault();
                obj.Documents = HttpContext.Request.Form["Documents"].FirstOrDefault();
                obj.DocumentType = HttpContext.Request.Form["DocumentType"].ToList();
                obj.IsDelmaIsland = Convert.ToBoolean(HttpContext.Request.Form["IsDelmaIsland"].FirstOrDefault());
                obj.IsUAEId = Convert.ToBoolean(HttpContext.Request.Form["IsUAEId"].FirstOrDefault());
                obj.CreatedOn = DateTime.UtcNow.AddHours(UTCHours);

                var res = _service.Add(obj);
                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {
                    // Set New Entry In Cache

                    response.Data = res;
                    response.ResponseMsg = "Data saved successfully!";
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

        // Register Group
        [AllowAnonymous]
        [HttpPost("AddGroup")]
        public Response AddGroup([FromForm] Register obj)
        {
            Response response = new Response();
            try
            {
                var data = JsonConvert.DeserializeObject<List<RegisterGroup>>(HttpContext.Request.Form["Group"]);

                foreach (IFormFile photo in HttpContext.Request.Form.Files)
                {
                    var supportedTypes = new[] { "doc", "docx", "jpeg", "jpg", "png", "pdf" };
                    var fileExt = Path.GetExtension(photo.FileName).Substring(1).ToLower();

                    if (!supportedTypes.Contains(fileExt))
                    {
                        response = CustomStatusResponse.GetResponse(600);
                        response.ResponseMsg = "Invalid file extension.";
                        return response;
                    }
                }

                var images = 1;

                for (int j = 0; j < data.Count; j++)
                {
                    data[j].FileName = new List<string>();

                    for (int i = 0; i < HttpContext.Request.Form.Files.Where(x => x.Name.Contains(("Group" + (j + 1)))).Count(); i++)
                    {
                        var file = HttpContext.Request.Form.Files["Group" + (j + 1) + "Image" + (images)];
                        var GuidFileName = Guid.NewGuid().ToString("n") + file.FileName;
                        var path = Path.Combine(this._env.WebRootPath, "Documents", GuidFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        data[j].FileName.Add("https://hajzapi.ada.ae/Documents/" + GuidFileName);
                        images++;
                    }
                }
                var encryptedPassword = _encryptionService.Encrypt(HttpContext.Request.Form["Password"].FirstOrDefault());
                obj.Username = HttpContext.Request.Form["Username"].FirstOrDefault();
                obj.Password = encryptedPassword;
                obj.Groups = data.ToList();
                obj.IsDelmaIsland = Convert.ToBoolean(HttpContext.Request.Form["IsDelmaIsland"].FirstOrDefault());
                obj.IsUAEId = Convert.ToBoolean(HttpContext.Request.Form["IsUAEId"].FirstOrDefault());
                obj.CreatedOn = DateTime.UtcNow.AddHours(UTCHours);

                var res = _service.AddGroup(obj);
                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {
                    response.Data = res;
                    response.ResponseMsg = "Data saved successfully!";
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

        // Register Corporate
        [AllowAnonymous]
        [HttpPost("AddCorporate")]
        public Response AddCorporate([FromForm] Register obj)
        {
            Response response = new Response();
            try
            {
                var data = JsonConvert.DeserializeObject<List<RegisterCorporate>>(HttpContext.Request.Form["Group"]);

                foreach (IFormFile photo in HttpContext.Request.Form.Files)
                {
                    var supportedTypes = new[] { "doc", "docx", "jpeg", "jpg", "png", "pdf" };
                    var fileExt = Path.GetExtension(photo.FileName).Substring(1).ToLower();

                    if (!supportedTypes.Contains(fileExt))
                    {
                        response = CustomStatusResponse.GetResponse(600);
                        response.ResponseMsg = "Invalid file extension.";
                        return response;
                    }
                }

                var images = 1;

                for (int j = 0; j < data.Count; j++)
                {
                    data[j].FileName = new List<string>();

                    for (int i = 0; i < HttpContext.Request.Form.Files.Where(x => x.Name.Contains(("Group" + (j + 1)))).Count(); i++)
                    {
                        var file = HttpContext.Request.Form.Files["Group" + (j + 1) + "Image" + (images)];
                        var GuidFileName = Guid.NewGuid().ToString("n") + file.FileName;
                        var path = Path.Combine(this._env.WebRootPath, "Documents", GuidFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        data[j].FileName.Add("https://hajzapi.ada.ae/Documents/" + GuidFileName);
                        images++;
                    }
                }
                var encryptedPassword = _encryptionService.Encrypt(HttpContext.Request.Form["Password"].FirstOrDefault());
                obj.Username = HttpContext.Request.Form["Username"].FirstOrDefault();
                obj.Password = encryptedPassword;
                obj.GovEntity = Convert.ToBoolean(HttpContext.Request.Form["GovEntity"].FirstOrDefault());
                obj.IsDelmaIsland = Convert.ToBoolean(HttpContext.Request.Form["IsDelmaIsland"].FirstOrDefault());
                obj.IsUAEId = Convert.ToBoolean(HttpContext.Request.Form["IsUAEId"].FirstOrDefault());
                obj.registerCorporate = data.ToList();
                obj.CreatedOn = DateTime.UtcNow.AddHours(UTCHours);

                var res = _service.AddCorporate(obj);
                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {
                    response.Data = res;
                    response.ResponseMsg = "Data saved successfully!";
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

       // [Authorize]
        [HttpPost("GetAllGroupUsersByID")]

        public Response GetAllGroupUsersByID(int Id)
        {
            Response response = new Response();

            try
            {
                var res = _service.GetAllGroupUsersById(Id);

                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {


                    response.Data = res;
                    response.ResponseMsg = "Get Of Group Data  successfully!";
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
        [HttpPost("GetAllFlightMembersByUserID")]
        public Response GetAllFlightMembersByUserID(int Id)
        {
            Response response = new Response();

            try
            {
                var res = _service.GetAllFlightMembersByUserID(Id);

                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {


                    response.Data = res;
                    response.ResponseMsg = "Get Of Group Data  successfully!";
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
        [HttpPost("MyprofileMembersByID")]
        public Pagination MyprofileMembersByID(int Id)
        {

            Response response = new Response();

            try
            {


                var draw = Request.Form["draw"].FirstOrDefault();


                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault(); 
                int pageSize = length != null ? Convert.ToInt32(length) : 0;

                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var Data = _service.GetAllGroupUsersById(Id);

       
                if (!string.IsNullOrEmpty(searchValue))
                {
                    searchValue = searchValue.ToString().ToLower();

                    Data = Data.Where(m => m.Name == null ? false : m.Name.ToString().ToLower().Contains(searchValue)

                   )?.ToList();
                }

                recordsTotal = Data.Count();

                if (sortColumn == "name" && sortColumnDir == "desc")
                {
                    Data = Data.OrderByDescending(d => d.Name).ToList();

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
        [HttpPost("GetSingleMemberById")]
        public Response GetSingleMemberById(UserIdWithType obj)
        {

            Response response = new Response();

            try
            {
                var res = _service.GetSingleMemberById(obj);

                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {


                    response.Data = res;
                    response.ResponseMsg = "Get Of Group Data  successfully!";
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

        // Update Members
        [Authorize]
        [HttpPost("UpdateMembers")]
        public Response UpdateMembers([FromForm] Register obj)
        {
            Response response = new Response();
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var files = Request.Form.Files;
                    obj.FileName = new List<string>();

                    foreach (IFormFile photo in files)
                    {
                        var GuidFileName = Guid.NewGuid().ToString("n") + photo.FileName;
                        var path = Path.Combine(this._env.WebRootPath, "Documents", GuidFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            photo.CopyTo(stream);
                        }

                        obj.FileName.Add("https://hajzapi.ada.ae/Documents/" + GuidFileName);
                    }
                }

                obj.UserId = Convert.ToInt32(HttpContext.Request.Form["UserId"].FirstOrDefault());
                obj.Id = Convert.ToInt32(HttpContext.Request.Form["Id"].FirstOrDefault());
                obj.MemberType = Convert.ToString(HttpContext.Request.Form["MemberType"].FirstOrDefault());
                obj.Honorifics = HttpContext.Request.Form["Honorifics"].FirstOrDefault();
                obj.Name = HttpContext.Request.Form["Name"].FirstOrDefault();
                obj.Email = HttpContext.Request.Form["Email"].FirstOrDefault();
                obj.Nationality = HttpContext.Request.Form["Nationality"].FirstOrDefault();
                obj.Username = HttpContext.Request.Form["Username"].FirstOrDefault();
                //obj.Password = HttpContext.Request.Form["Password"].FirstOrDefault();
                obj.Birthdate = Convert.ToDateTime(HttpContext.Request.Form["Birthdate"].FirstOrDefault());
                obj.CreatedOn = DateTime.Now;
                obj.Mobile = HttpContext.Request.Form["Mobile"].FirstOrDefault();
                obj.Documents = HttpContext.Request.Form["Documents"].FirstOrDefault();
                obj.DocumentType = HttpContext.Request.Form["DocumentType"].ToList();
                obj.IsDelmaIsland = Convert.ToBoolean(HttpContext.Request.Form["IsDelmaIsland"].FirstOrDefault());
                obj.GovEntity = Convert.ToBoolean(HttpContext.Request.Form["GovEntity"].FirstOrDefault());
                obj.IsUAEId = Convert.ToBoolean(HttpContext.Request.Form["IsUAEId"].FirstOrDefault());
                obj.CreatedOn = DateTime.UtcNow.AddHours(UTCHours);

                var res = _service.UpdateMembers(obj);
                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {
                    response.Data = res;
                    response.ResponseMsg = "Data saved successfully!";
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

        [AllowAnonymous]
        [HttpPost("AddOtherGroupMember")]
        public Response AddOtherGroupMember([FromForm] Register obj)
        {
            Response response = new Response();
            try
            {
                var data = JsonConvert.DeserializeObject<List<RegisterGroup>>(HttpContext.Request.Form["GroupData"]);

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var files = Request.Form.Files;
                    data[0].FileName = new List<string>();

                    foreach (IFormFile photo in files)
                    {
                        var supportedTypes = new[] { "doc", "docx", "jpeg", "jpg", "png", "pdf" };
                        var fileExt = System.IO.Path.GetExtension(photo.FileName).Substring(1).ToLower();

                        if (!supportedTypes.Contains(fileExt))
                        {
                            response = CustomStatusResponse.GetResponse(600);
                            response.ResponseMsg = "Invalid file extension.";
                            return response;
                        }
                    }

                    foreach (IFormFile photo in files)
                    {
                        var GuidFileName = Guid.NewGuid().ToString("n") + photo.FileName;
                        var path = Path.Combine(this._env.WebRootPath, "Documents", GuidFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            photo.CopyTo(stream);
                        }

                        data[0].FileName.Add("https://hajzapi.ada.ae/Documents/" + GuidFileName);
                    }
                }

                obj.Groups = data;

                var res = _service.AddOtherGroupMember(obj);
                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {
                    response.Data = res;
                    response.ResponseMsg = "Member registered successfully.";
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

        [AllowAnonymous]
        [HttpPost("AddOtherCorporateMember")]
        public Response AddOtherCorporateMember([FromForm] Register obj)
        {
            Response response = new Response();
            try
            {
                var data = JsonConvert.DeserializeObject<List<RegisterCorporate>>(HttpContext.Request.Form["GroupData"]);

                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var files = Request.Form.Files;
                    data[0].FileName = new List<string>();

                    foreach (IFormFile photo in files)
                    {
                        var supportedTypes = new[] { "doc", "docx", "jpeg", "jpg", "png", "pdf" };
                        var fileExt = System.IO.Path.GetExtension(photo.FileName).Substring(1).ToLower();

                        if (!supportedTypes.Contains(fileExt))
                        {
                            response = CustomStatusResponse.GetResponse(600);
                            response.ResponseMsg = "Invalid file extension.";
                            return response;
                        }
                    }

                    foreach (IFormFile photo in files)
                    {
                        var GuidFileName = Guid.NewGuid().ToString("n") + photo.FileName;
                        var path = Path.Combine(this._env.WebRootPath, "Documents", GuidFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            photo.CopyTo(stream);
                        }

                        data[0].FileName.Add("https://hajzapi.ada.ae/Documents/" + GuidFileName);
                    }
                }

                obj.registerCorporate = data;

                var res = _service.AddOtherCorporateMember(obj);
                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {
                    response.Data = res;
                    response.ResponseMsg = "Member registered successfully.";
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
        [HttpPost("DeleteGroupMember/{Id}")]
        public Response DeleteGroupMember(int Id)
        {


            Response response = new Response();

            try
            {
                var res = _service.DeleteGroupMember(Id);

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
        [HttpPost("DeleteCorporateMember/{Id}")]
        public Response DeleteCorporateMember(int Id)
        {


            Response response = new Response();

            try
            {
                var res = _service.DeleteCorporateMember(Id);

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

       
        [HttpPost("Update")]
        public Response Update([FromForm] Register obj)
        {
            Response response = new Response();
            try
            {
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var files = Request.Form.Files;
                    obj.FileName = new List<string>();

                    foreach (IFormFile photo in files)
                    {
                        var supportedTypes = new[] { "doc", "docx", "jpeg", "jpg", "png", "pdf" };
                        var fileExt = System.IO.Path.GetExtension(photo.FileName).Substring(1).ToLower();

                        if (!supportedTypes.Contains(fileExt))
                        {
                            response = CustomStatusResponse.GetResponse(600);
                            response.ResponseMsg = "Invalid file extension.";
                            return response;
                        }
                    }

                    var oldFilePaths = HttpContext.Request.Form["fileInput"].ToList();
                    var fileIndex = 0;

                    for (int i = 0; i < oldFilePaths.Count(); i++)
                    {
                        if (oldFilePaths[i] != "null")
                        {
                            obj.FileName.Add("https://hajzapi.ada.ae/Documents/" + oldFilePaths[i]);
                        }
                        else
                        {
                            var file = files[fileIndex];
                            var GuidFileName = Guid.NewGuid().ToString("n") + file.FileName;
                            var path = Path.Combine(this._env.WebRootPath, "Documents", GuidFileName);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            obj.FileName.Add("https://hajzapi.ada.ae/Documents/" + GuidFileName);
                            fileIndex++;
                        }
                    }
                }
                else
                {
                    var fileNameOld = new List<string>();
                    var oldFilePath = HttpContext.Request.Form["fileInput"].ToList();

                    for (int i = 0; i < oldFilePath.Count(); i++)
                    {
                        var oldFileName = "https://hajzapi.ada.ae/Documents/" + oldFilePath[i];
                        fileNameOld.Add(oldFileName);
                    }

                    obj.FileName = fileNameOld;
                }
                var encryptedPassword = _encryptionService.Encrypt(HttpContext.Request.Form["Password"].FirstOrDefault());

                obj.Id = Convert.ToInt32(HttpContext.Request.Form["Id"].FirstOrDefault());
                obj.Honorifics = HttpContext.Request.Form["Honorifics"].FirstOrDefault();
                obj.Name = HttpContext.Request.Form["Name"].FirstOrDefault();
                obj.Email = string.IsNullOrEmpty(HttpContext.Request.Form["Email"].FirstOrDefault())
                        ? HttpContext.Request.Form["Mobile"].FirstOrDefault() + "@dummy.ada.ae"
                        : obj.Email;
                obj.Nationality = HttpContext.Request.Form["Nationality"].FirstOrDefault();
                obj.Username = HttpContext.Request.Form["Username"].FirstOrDefault();
                obj.Password = encryptedPassword;
                obj.Birthdate = Convert.ToDateTime(HttpContext.Request.Form["Birthdate"].FirstOrDefault());
                obj.Mobile = HttpContext.Request.Form["Mobile"].FirstOrDefault();
                obj.Documents = HttpContext.Request.Form["Documents"].FirstOrDefault();
                obj.DocumentType = HttpContext.Request.Form["DocumentType"].ToList();
                obj.IsDelmaIsland = Convert.ToBoolean(HttpContext.Request.Form["IsDelmaIsland"].FirstOrDefault());
                obj.IsUAEId = Convert.ToBoolean(HttpContext.Request.Form["IsUAEId"].FirstOrDefault());
                obj.CreatedOn = DateTime.UtcNow.AddHours(UTCHours);

                var res = _service.update(obj);
                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {
                    // Set New Entry In Cache
                    response.Data = res;
                    response.ResponseMsg = "Data saved successfully!";
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

        
        [HttpPost("UpdateGroup")]
        public Response UpdateGroup([FromForm] Register obj)
        {
            Response response = new Response();
            try
            {
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var files = Request.Form.Files;
                    obj.FileName = new List<string>();

                    foreach (IFormFile photo in files)
                    {
                        var supportedTypes = new[] { "doc", "docx", "jpeg", "jpg", "png", "pdf" };
                        var fileExt = System.IO.Path.GetExtension(photo.FileName).Substring(1).ToLower();

                        if (!supportedTypes.Contains(fileExt))
                        {
                            response = CustomStatusResponse.GetResponse(600);
                            response.ResponseMsg = "Invalid file extension.";
                            return response;
                        }
                    }

                    var oldFilePaths = HttpContext.Request.Form["fileInput"].ToList();
                    var fileIndex = 0;

                    for (int i = 0; i < oldFilePaths.Count(); i++)
                    {
                        if (oldFilePaths[i] != "null")
                        {
                            obj.FileName.Add("https://hajzapi.ada.ae/Documents/" + oldFilePaths[i]);
                        }
                        else
                        {
                            var file = files[fileIndex];
                            var GuidFileName = Guid.NewGuid().ToString("n") + file.FileName;
                            var path = Path.Combine(this._env.WebRootPath, "Documents", GuidFileName);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            obj.FileName.Add("https://hajzapi.ada.ae/Documents/" + GuidFileName);
                            fileIndex++;
                        }
                    }
                }
                else
                {
                    var fileNameOld = new List<string>();
                    var oldFilePath = HttpContext.Request.Form["fileInput"].ToList();

                    for (int i = 0; i < oldFilePath.Count(); i++)
                    {
                        var oldFileName = "https://hajzapi.ada.ae/Documents/" + oldFilePath[i];
                        fileNameOld.Add(oldFileName);
                    }

                    obj.FileName = fileNameOld;
                }

                
                obj.Id = Convert.ToInt32(HttpContext.Request.Form["Id"].FirstOrDefault());
                obj.Honorifics = HttpContext.Request.Form["Honorifics"].FirstOrDefault();
                obj.Name = HttpContext.Request.Form["Name"].FirstOrDefault();
                obj.Email = HttpContext.Request.Form["Email"].FirstOrDefault();
                obj.Nationality = HttpContext.Request.Form["Nationality"].FirstOrDefault();
                obj.Birthdate = Convert.ToDateTime(HttpContext.Request.Form["Birthdate"].FirstOrDefault());
                obj.Mobile = HttpContext.Request.Form["Mobile"].FirstOrDefault();
                obj.Documents = HttpContext.Request.Form["Documents"].FirstOrDefault();
                obj.DocumentType = HttpContext.Request.Form["DocumentType"].ToList();
                obj.IsDelmaIsland = Convert.ToBoolean(HttpContext.Request.Form["IsDelmaIsland"].FirstOrDefault());
                obj.IsHead = Convert.ToBoolean(HttpContext.Request.Form["IsHead"].FirstOrDefault());
                obj.IsUAEId = Convert.ToBoolean(HttpContext.Request.Form["IsUAEId"].FirstOrDefault());
                obj.CreatedOn = DateTime.UtcNow.AddHours(UTCHours);

                var res = _service.updateGroup(obj);
                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {
                    // Set New Entry In Cache
                    response.Data = res;
                    response.ResponseMsg = "Data saved successfully!";
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

        
        [HttpPost("UpdateCorporate")]
        public Response UpdateCorporate([FromForm] Register obj)
        {
            Response response = new Response();
            try
            {
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var files = Request.Form.Files;
                    obj.FileName = new List<string>();

                    foreach (IFormFile photo in files)
                    {
                        var supportedTypes = new[] { "doc", "docx", "jpeg", "jpg", "png", "pdf" };
                        var fileExt = System.IO.Path.GetExtension(photo.FileName).Substring(1).ToLower();

                        if (!supportedTypes.Contains(fileExt))
                        {
                            response = CustomStatusResponse.GetResponse(600);
                            response.ResponseMsg = "Invalid file extension.";
                            return response;
                        }
                    }

                    var oldFilePaths = HttpContext.Request.Form["fileInput"].ToList();
                    var fileIndex = 0;

                    for (int i = 0; i < oldFilePaths.Count(); i++)
                    {
                        if (oldFilePaths[i] != "null")
                        {
                            obj.FileName.Add("https://hajzapi.ada.ae/Documents/" + oldFilePaths[i]);
                        }
                        else
                        {
                            var file = files[fileIndex];
                            var GuidFileName = Guid.NewGuid().ToString("n") + file.FileName;
                            var path = Path.Combine(this._env.WebRootPath, "Documents", GuidFileName);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            obj.FileName.Add("https://hajzapi.ada.ae/Documents/" + GuidFileName);
                            fileIndex++;
                        }
                    }
                }
                else
                {
                    var fileNameOld = new List<string>();
                    var oldFilePath = HttpContext.Request.Form["fileInput"].ToList();

                    for (int i = 0; i < oldFilePath.Count(); i++)
                    {
                        var oldFileName = "https://hajzapi.ada.ae/Documents/" + oldFilePath[i];
                        fileNameOld.Add(oldFileName);
                    }

                    obj.FileName = fileNameOld;
                }

                obj.Id = Convert.ToInt32(HttpContext.Request.Form["Id"].FirstOrDefault());
                obj.Honorifics = HttpContext.Request.Form["Honorifics"].FirstOrDefault();
                obj.Name = HttpContext.Request.Form["Name"].FirstOrDefault();
                obj.Email = HttpContext.Request.Form["Email"].FirstOrDefault();
                obj.Nationality = HttpContext.Request.Form["Nationality"].FirstOrDefault();
                obj.Birthdate = Convert.ToDateTime(HttpContext.Request.Form["Birthdate"].FirstOrDefault());
                obj.Mobile = HttpContext.Request.Form["Mobile"].FirstOrDefault();
                obj.Documents = HttpContext.Request.Form["Documents"].FirstOrDefault();
                obj.DocumentType = HttpContext.Request.Form["DocumentType"].ToList();
                obj.IsDelmaIsland = Convert.ToBoolean(HttpContext.Request.Form["IsDelmaIsland"].FirstOrDefault());
                obj.GovEntity = Convert.ToBoolean(HttpContext.Request.Form["GovEntity"].FirstOrDefault());
                obj.IsHead = Convert.ToBoolean(HttpContext.Request.Form["IsHead"].FirstOrDefault());
                obj.IsUAEId = Convert.ToBoolean(HttpContext.Request.Form["IsUAEId"].FirstOrDefault());
                obj.CreatedOn = DateTime.UtcNow.AddHours(UTCHours);

                var res = _service.updateCorporate(obj);
                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {
                    // Set New Entry In Cache
                    response.Data = res;
                    response.ResponseMsg = "Data saved successfully!";
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
        [AuthorizeUserIdDocument("Id")]
        [HttpPost("GetDocumentByUserId/{Id}")]
        public Response GetDocumentByUserId(int Id)
        {
            Response response = new Response();

            try
            {
                if(!(int.Parse(User.FindFirst("id")?.Value)==Id)) return CustomStatusResponse.GetResponse(401);

                var res = _service.GetDocumentByUserId(Id);

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
        [AuthorizeUserIdDocument("Id")]
        [HttpPost("GetGroupDocumentByUserId/{Id}")]
        public Response GetGroupDocumentByUserId(int Id)
        {
            Response response = new Response();

            try
            {
                var res = _service.GetGroupDocumentByUserId(Id);

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
        [AuthorizeUserIdDocument("Id")]
        [HttpPost("GetCorporateDocumentByUserId/{Id}")]
        public Response GetCorporateDocumentByUserId(int Id)
        {
            Response response = new Response();

            try
            {
                var res = _service.GetCorporateDocumentByUserId(Id);

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

        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public Response ForgotPassword([FromForm] CodeVerification obj)
        {
            Response response = new Response();
            try
            {
                obj.Email = HttpContext.Request.Form["Email"].FirstOrDefault();
                obj.Mobile = HttpContext.Request.Form["Mobile"].FirstOrDefault();
                obj.Action = HttpContext.Request.Form["Action"].FirstOrDefault().Replace('#', ' ').Trim();

                if (obj.Email.Contains("@dummy.ada.ae"))
                {
                    response = CustomStatusResponse.GetResponse(700);
                    response.ResponseMsg = "The email you entered is invalid. Please enter a valid email or use SMS for verification instead.";
                    return response;
                }


                var res = _service.ForgotPassword(obj);
                response = CustomStatusResponse.GetResponse(200);



                if (res != null)
                {

                    if (res.Mobile!=null)
                    {
                        sendSMS(res, sync: true).GetAwaiter().GetResult();

                        response.ResponseMsg = "Verification code has been send to the number you provided";
                    }

                    else if (!res.Email.Contains("@dummy.ada.ae") && res.Email!=null)
                    {
                        response.Data = res.Email;
                        response.ResponseMsg = "Link to reset your password has been sent to the registered email.";

                        // Get email server settings from appsettings.json
                        IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();
                        string smtpServer = configuration["EmailSettings:SmtpServer"];
                        int smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
                        string smtpUsername = configuration["EmailSettings:SmtpUsername"];
                        string smtpPassword = configuration["EmailSettings:SmtpPassword"];
                        string fromAddress = configuration["EmailSettings:FromAddress"];

                        string to = res.Email;
                        string from = fromAddress;
                        MailMessage message = new MailMessage(from, to);
                        //string mailbody = "Your Verification Code is " + res.VerifyCode;
                        string mailbody = $"Dear User,<br><br>We have received a request to reset your password for Abu Dhabi Aviation. To ensure the security of your account, we require you to verify your identity before proceeding.<br><br>Please use the following verification code to confirm your account ownership and reset your password:<br><br><strong>Verification code:</strong> {res.VerifyCode}<br><br>Please note that this code is valid for a limited time and can only be used once. If you did not initiate this request, please ignore this email.<br><br>To reset your password, please copy the verification code and follow the instructions:<br>{res.VerifyCode}<br><br>If you experience any issues, please do not hesitate to contact our support team.<br><br>Thank you for using Abu Dhabi Aviation.<br><br>Best regards,<br>Thanks<br>Abu Dhabi Aviation Support Team";
                        message.Subject = "Reset Password";
                        message.Body = mailbody;
                        message.BodyEncoding = Encoding.UTF8;
                        message.IsBodyHtml = true;

                        SmtpClient client = new SmtpClient(smtpServer, smtpPort);
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                        try
                        {
                            client.Send(message);
                        }
                        catch (SmtpException ex)
                        {
                            Console.WriteLine("SMTP Error: " + ex.StatusCode + " " + ex.Message);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                            throw;
                        }
                    }


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
                // Log the error details
                Console.WriteLine("Error sending email: " + ex.ToString());
                response = CustomStatusResponse.GetResponse(500);
                response.ResponseMsg = ex.Message;
                return response;
            }
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public Response ResetPassword([FromForm] CodeVerification obj)
        {

            Response response = new Response();
            try
            {

                var encryptedPassword = _encryptionService.Encrypt(HttpContext.Request.Form["Password"].FirstOrDefault());

                obj.VerifyCode = HttpContext.Request.Form["VerifyCode"].FirstOrDefault();
                obj.Email = HttpContext.Request.Form["Email"].FirstOrDefault();
                obj.Mobile = HttpContext.Request.Form["Mobile"].FirstOrDefault();
                obj.Action = HttpContext.Request.Form["Action"].FirstOrDefault().Replace('#', ' ').Trim();
                obj.Password = encryptedPassword;
               
               

              

                var res = _service.ResetPassword(obj);
                response = CustomStatusResponse.GetResponse(200);

                if (res != null)
                {

                    #region Set New Entry In Cache


                    #endregion

                    response.Data = res;
                    response.ResponseMsg = "Password changed Successfuly!";
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
        [HttpPost("ActiveInactiveByUserId/{Id}")]
        public Response ActiveInactiveByUserId(int Id)
        {
            Response response = new Response();

            try
            {
                var res = _service.ActiveInactiveByUserId(Id);

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

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<string>> sendSMS(dynamic res, bool sync)
        {
            
            var forgotPasswordMessage = $"Please use the following verification code to confirm your account ownership and reset your password. Your Verification code is {res.VerifyCode}. Please note that this code is valid for a limited time and can only be used once."; 
            


            

           var urlobj = "https://apiw.me.synapselive.com/push.aspx?user=AbuDhabiADAIT&pass=ad@adait&senderid=ADA&lang=0&mobile=971" + res.Mobile + "&lang=0&message=" + forgotPasswordMessage + "&dlr=1";


            using (HttpClient client = new HttpClient())
            {

                    await client.GetStringAsync(urlobj);

            }

            return null;
        }
    }
}
