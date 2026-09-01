using socialmedia.DTOs.Users.Request;
using socialmedia.DTOs.Users.Response;
using socialmedia.Repositories;
using socialmedia.Repositories.UserRepostories;
using socialmedia.Services.Interfaces;


namespace socialmedia.Services
{
    public class UserService:IUserService
    {
        private readonly UsersRepository _usersRepository;
        private readonly FollowRepository _followRepository;
        private readonly PostRepository _postRepository;

        public UserService(UsersRepository usersRepository, FollowRepository followRepository, PostRepository postRepository)
        {
            _usersRepository = usersRepository;
            _followRepository = followRepository;
            _postRepository = postRepository;
        }

        public async Task<MyProfileDto> GetMyProfileAsync(long userId)
        {
            var data = await _usersRepository.GetUserFullDataAsync(userId);
            if (data == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            int postCount = await _postRepository.GetPostCountByUserIdAsync(userId);

            return new MyProfileDto
            {
                UserID = data.User.UserID,
                Username = data.User.Username,
                Email = data.User.Email,
                FirstName = data.Profile.FirstName,
                LastName = data.Profile.LastName,
                Biography = data.Profile.Biography,
                PPUrl = data.Profile.PPUrl,
                BirthDate = data.Profile.BirthDate,
                IsVerified = data.Settings.IsVerified,
                IsPrivate = data.Settings.IsPrivate,
                FollowerCount = data.Profile.FollowerCount,
                FollowingCount = data.Profile.FollowingCount,
                PostCount = postCount
            };
        }

        public async Task<UserProfileDto> GetUserProfileAsync(long targetUserId, long currentUserId)
        {
            var data = await _usersRepository.GetUserFullDataAsync(targetUserId);
            if (data == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            int postCount = await _postRepository.GetPostCountByUserIdAsync(targetUserId);
            bool isFollowed = await _followRepository.IsFollowingAsync(currentUserId, targetUserId);

            return new UserProfileDto
            {
                UserID = data.User.UserID,
                Username = data.User.Username,
                FirstName = data.Profile.FirstName,
                LastName = data.Profile.LastName,
                Biography = data.Profile.Biography,
                PPUrl = data.Profile.PPUrl,
                IsVerified = data.Settings.IsVerified,
                IsPrivate = data.Settings.IsPrivate,
                FollowerCount = data.Profile.FollowerCount,
                FollowingCount = data.Profile.FollowingCount,
                PostCount = postCount,
                IsFollowedByCurrentUser = isFollowed
            };
        }

        public async Task UpdateUserProfileAsync(long userId, UpdateProfileDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new ArgumentException("Ad alanı boş bırakılamaz.");
            if (string.IsNullOrWhiteSpace(dto.LastName))
                throw new ArgumentException("Soyad alanı boş bırakılamaz.");

            await _usersRepository.UpdateProfileAsync(userId, dto.FirstName, dto.LastName, dto.Biography, dto.PPUrl, dto.BirthDate);
        }

        public async Task UpdateUserSettingsAsync(long userId, UpdateSettingsDto dto)
        {
            await _usersRepository.UpdatePrivacyAsync(userId, dto.IsPrivate);
        }

        public async Task FreezeAccountAsync(long userId) => await _usersRepository.UpdateStatusAsync(userId, false);
        public async Task ActivateAccountAsync(long userId) => await _usersRepository.UpdateStatusAsync(userId, true);
        public async Task DeleteAccountAsync(long userId) => await _usersRepository.DeleteUserAsync(userId, DateTime.UtcNow);
    }
}
