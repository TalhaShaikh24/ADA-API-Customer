using ADA.API.DBManager;
using ADA.API.Helpers;
using ADA.API.IRepositories;
using ADA.API.IServices;
using ADA.API.Repositories;
using ADAClassLibrary;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;

namespace ADA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionController : ControllerBase
    {
        private readonly EncryptionService _encryptionService;
        private readonly IDapper _dapper;

        public EncryptionController(EncryptionService encryptionService, IDapper dapper)
        {
            _encryptionService = encryptionService;
            _dapper = dapper;
        }

        //[HttpPost("encrypt")]
        //public IActionResult Encrypt([FromBody] string password)
        //{
        //    if (string.IsNullOrEmpty(password))
        //        return BadRequest("Password is required.");

        //    var encrypted = _encryptionService.Encrypt(password);
        //    return Ok(new { EncryptedPassword = encrypted });
        //}




        [HttpPost("EncryptAllPasswords")]
        public IActionResult EncryptPassword()
        {
            DynamicParameters parameters = new DynamicParameters();

            var data = _dapper.GetAll<Passwords>(@"[dbo].[GetAllPasswords]", parameters);


            foreach (var item in data)
            {
                var ep = _encryptionService.Encrypt(item.Password);
                item.Password = ep;
                _encryptionService.EncryptPassword(item);
            }
            return Ok();
        }

        [HttpPost("DecryptAllPasswords")]
        public IActionResult DecryptAllPasswords()
        {
            DynamicParameters parameters = new DynamicParameters();

            var data = _dapper.GetAll<Passwords>(@"[dbo].[GetAllPasswords]", parameters);


            foreach (var item in data)
            {
                var ep = _encryptionService.Decrypt(item.Password);
                item.Password = ep;
                _encryptionService.EncryptPassword(item);
            }
            return Ok();
        }

    }

   
}
