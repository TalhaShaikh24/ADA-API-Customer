using ADA.API.IRepositories;
using ADA.API.Repositories;
using ADA.IServices;
using ADAClassLibrary;
using ADAClassLibrary.DTOLibraries;
using System;
using System.Threading.Tasks;

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
        public async Task<Register> GetUserByIdAsync(int userId)
        {
            return await _repository.GetUserByIdAsync(userId);
        }


        public Task<string> GetSavedTokenAsync(int userId)
        {
            return _repository.GetSavedTokenAsync(userId);
        }

        public Task SaveUserToken(int userId, string token, DateTime issuedAt, DateTime ExpiryDate)
        {
            return _repository.SaveUserToken(userId,token,issuedAt,ExpiryDate);
        }

        public Task<bool> IsTokenValidAsync(int userId, string token)
        {
            return _repository.IsTokenValidAsync(userId, token);
        }

        public Task LogoutAsync(int userId, string token)
        {
            return _repository.LogoutAsync(userId,token);
        }
    }
}
