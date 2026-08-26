using socialmedia.DataTransferObject;
using socialmedia.Repositories;

namespace socialmedia.Service
{
    public class FollowService
    {
        private readonly FollowRepository _followRepository;
        public FollowService(FollowRepository followRepository)
        {
            _followRepository = followRepository;
        }

        public async Task FollowUserAsync(int followerID, int followingID)
        {
            if (followerID == followingID)
            {
                throw new ArgumentException("Kendini takip edemezsin.");
            }

            bool SuccessFollow = await _followRepository.FollowUserAsync(followerID, followingID);

            if (!SuccessFollow)
            {
                throw new Exception("Bu kullanıcıyı zaten takip ediyorsun veya bekleyen bir isteğin var.");
            }
        }
    }
}
