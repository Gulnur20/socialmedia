using socialmedia.DTOs.Users.Request;
using socialmedia.DTOs.Users.Response;

namespace socialmedia.Services.Interfaces
{
    public interface IUserService
    {
        Task<MyProfileDto> GetMyProfileAsync(long userId);
        Task<UserProfileDto> GetUserProfileAsync(long targetUserId, long currentUserId);
        Task UpdateUserProfileAsync(long userId, UpdateProfileDto dto);
        Task UpdateUserSettingsAsync(long userId, UpdateSettingsDto dto);
        Task FreezeAccountAsync(long userId);
        Task ActivateAccountAsync(long userId);
        Task DeleteAccountAsync(long userId);
    }
}
