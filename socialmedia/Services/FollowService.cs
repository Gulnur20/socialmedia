using socialmedia.DTOs;
using socialmedia.DTOs.Follow.Response;
using socialmedia.Exceptions;
using socialmedia.Repositories;
using socialmedia.Services.Interfaces;

namespace socialmedia.Services
{
    public class FollowService:IFollowService
    {
        private readonly FollowRepository _followRepository;

        public FollowService(FollowRepository followRepository)
        {
            _followRepository = followRepository;
        }

        public async Task<FollowActionResultDto> FollowUserAsync(long followerId, long followingId)
        {
            if (followerId == followingId)
                throw new ArgumentException("Kendini takip edemezsin.");

            bool alreadyFollowing = await _followRepository.IsFollowingAsync(followerId, followingId);
            if (alreadyFollowing)
                throw new AlreadyFollowingException("Bu kullanıcıyı zaten takip ediyorsun.");

            string status = await _followRepository.FollowUserAsync(followerId, followingId);

            return new FollowActionResultDto { Status = status };
        }

        public async Task UnfollowAsync(long followerId, long followingId)
        {
            bool success = await _followRepository.UnfollowAsync(followerId, followingId);
            if (!success)
                throw new KeyNotFoundException("Zaten takip etmiyorsun.");
        }

        public async Task<List<FollowRequestDto>> GetWaitingFollowRequestsAsync(long userId)
        {
            return await _followRepository.GetWaitingFollowRequestsAsync(userId);
        }

        public async Task AcceptFollowRequestAsync(long requestId)
        {
            bool success = await _followRepository.AcceptFollowRequestAsync(requestId);
            if (!success)
                throw new KeyNotFoundException("Takip isteği bulunamadı.");
        }

        public async Task RejectFollowRequestAsync(long requestId)
        {
            bool success = await _followRepository.RejectFollowRequestAsync(requestId);
            if (!success)
                throw new KeyNotFoundException("Takip isteği bulunamadı.");
        }
    }
}
