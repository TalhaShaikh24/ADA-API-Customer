using ADA.API.DBManager;
using ADAClassLibrary;
using Dapper;
using ADA.API.IRepositories;
using System.Data;
using ADAClassLibrary.DTOLibraries;
using System.Linq;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;


namespace ADA.API.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IDapper _dapper;

        public AuthenticationRepository(IDapper dapper)
        {
            _dapper = dapper;
        }
        public ClaimDTO Authenticate(LoginCredentials obj)
        {

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Username", obj.Username, DbType.String, ParameterDirection.Input);
            parameters.Add("Password", obj.Password, DbType.String, ParameterDirection.Input);

            var data = _dapper.Get<ClaimDTO>(@"[dbo].[usp_ValidateLogin]", parameters);
            ClaimDTO claimDTO = null;
            if (data != null)
            {
                claimDTO = new ClaimDTO();
                claimDTO = data;
            }
            return claimDTO;
        }

        public async Task<string> GetSavedTokenAsync(int userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.Get<string>(@"[dbo].[sp_GetSavedTokenAsync]", parameters);

            return data;
        }

        public async Task SaveUserToken(int userId, string token, DateTime issuedAt, DateTime ExpiryDate)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@token", token, DbType.String, ParameterDirection.Input);
            parameters.Add("@issuedAt", issuedAt, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@ExpiryDate", ExpiryDate, DbType.DateTime, ParameterDirection.Input);

            _dapper.Insert<string>(@"[dbo].[sp_SaveUserToken]", parameters);
        }

        public async Task<Register> GetUserByIdAsync(int userId)
        {
            DynamicParameters parameters = new DynamicParameters();
           
            parameters.Add("@userId", userId, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.Get<Register>(@"[dbo].[sp_GetUserByIdAsync]", parameters);

            return data;
        }

        public async Task<bool> IsTokenValidAsync(int userId, string token)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@token", token, DbType.String, ParameterDirection.Input);

           var data= _dapper.Get<bool>(@"[dbo].[sp_IsTokenValidAsync]", parameters);

            return data;
        }

        public async Task LogoutAsync(int userId, string token)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@token", token, DbType.String, ParameterDirection.Input);
            _dapper.Insert(@"[dbo].[sp_LogoutAsync]", parameters);
        }
    }
}
