using ADA.API.DBManager;
using ADAClassLibrary;
using Dapper;
using ADA.API.IRepositories;
using System.Data;
using ADAClassLibrary.DTOLibraries;
using System.Linq;


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

       
    }
}
