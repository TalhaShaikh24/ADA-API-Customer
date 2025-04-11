using ADAClassLibrary;
using ADAClassLibrary.DTOLibraries;
using System;
using System.Threading.Tasks;

namespace ADA.API.IRepositories
{
    public interface IAuthenticationRepository
    {
        ClaimDTO Authenticate(LoginCredentials obj);
        Task<Register> GetUserByIdAsync(int userId);
        Task<string> GetSavedTokenAsync(int userId);
        Task SaveUserToken(int userId, string token, DateTime issuedAt, DateTime ExpiryDate);
        Task<bool> IsTokenValidAsync(int userId, string token);
        Task LogoutAsync(int userId, string token);

    }
}
