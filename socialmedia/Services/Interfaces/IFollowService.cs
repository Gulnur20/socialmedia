using socialmedia.DTOs.Follow.Response;
using socialmedia.DTOs.Users.Response;

namespace socialmedia.Services.Interfaces
{
    public interface IFollowService
    {

        Task<FollowActionResultDto> FollowUserAsync(long followerId, long followingId);
        Task UnfollowAsync(long followerId, long followingId);
        Task<List<FollowRequestDto>> GetWaitingFollowRequestsAsync(long userId);
        Task AcceptFollowRequestAsync(long requestId);
        Task RejectFollowRequestAsync(long requestId);
        Task<List<UserSummaryDto>> GetFollowersAsync(long userId);
        Task<List<UserSummaryDto>> GetFollowingAsync(long userId);
    }
}
