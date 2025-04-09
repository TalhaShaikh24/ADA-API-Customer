
using ADAClassLibrary;
using ADAClassLibrary.DTOLibraries;

namespace ADA.IServices
{
    public interface IAuthenticationService
    {
        ClaimDTO Authenticate(LoginCredentials obj);
 
    }
}
