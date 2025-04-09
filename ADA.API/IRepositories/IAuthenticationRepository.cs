using ADAClassLibrary;
using ADAClassLibrary.DTOLibraries;

namespace ADA.API.IRepositories
{
    public interface IAuthenticationRepository
    {
        ClaimDTO Authenticate(LoginCredentials obj);
       
    }
}
