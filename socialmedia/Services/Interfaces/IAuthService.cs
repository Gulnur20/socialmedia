using socialmedia.DTOs.Auth;
using socialmedia.DTOs.Users.Response;

namespace socialmedia.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserSummaryDto> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto, string ipAddress);
    }
}
