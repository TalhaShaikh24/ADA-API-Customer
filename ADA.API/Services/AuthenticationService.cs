using ADA.API.IRepositories;
using ADA.IServices;
using ADAClassLibrary;
using ADAClassLibrary.DTOLibraries;

namespace ADA.API.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _repository;

        public AuthenticationService(IAuthenticationRepository repository)
        {
            _repository = repository;
        }
        public ClaimDTO Authenticate(LoginCredentials obj)
        {


            return _repository.Authenticate(obj);
        }

        
    }
}
