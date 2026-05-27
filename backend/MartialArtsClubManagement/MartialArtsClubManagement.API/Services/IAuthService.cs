using MartialArtsClubManagement.API.Models.DTOs;

namespace MartialArtsClubManagement.API.Services
{
    public interface IAuthService
    {
        LoginResponse Authenticate(LoginRequest request);
    }
}
