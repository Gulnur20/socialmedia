using socialmedia.DTOs.Auth;
using socialmedia.DTOs.Users.Response;
using socialmedia.Exceptions;
using socialmedia.Models;
using socialmedia.Repositories.UserRepostories;
using socialmedia.Services.Interfaces;

namespace socialmedia.Services
{
    public class AuthService:IAuthService
    {
        private readonly UsersRepository _usersRepository;
        private readonly ITokenService _tokenService;

        public AuthService(UsersRepository usersRepository, ITokenService tokenService)
        {
            _usersRepository = usersRepository;
            _tokenService = tokenService;
        }

        public async Task<UserSummaryDto> RegisterAsync(RegisterRequestDto dto)
        {
            int age = CalculateAge(dto.BirthDate);
            if (age < 15)
                throw new AgeRestrictionException("Kayıt olabilmek için en az 15 yaşında olmalısınız.");

            bool isUsernameTaken = await _usersRepository.CheckUsernameAsync(dto.Username);
            if (isUsernameTaken)
                throw new DuplicateUsernameException("Bu kullanıcı adı zaten kullanılıyor. Lütfen başka bir kullanıcı adı seçin.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new Users
            {
                UserID = 0,
                Username = dto.Username,
                Password = passwordHash,
                Email = dto.Email,
                IsActive = true,
                UserCreated = DateTime.UtcNow
            };

            var profile = new UserProfile
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Biography = null,
                PPUrl = null,
                BirthDate = dto.BirthDate
            };

            var settings = new UserSettings
            {
                IsPrivate = false,
                IsVerified = false,
                IsEmailConfirmed = false,
                FailedLoginCount = 0,
                LastUsernameChanged = DateTime.UtcNow,
                LastPasswordChanged = DateTime.UtcNow,
                LastLoginIP = null,
                LastLoginDate = DateTime.UtcNow,
                DeletedDate = null
            };

            await _usersRepository.AddUserAsync(user, profile, settings);

            return new UserSummaryDto
            {
                UserID = user.UserID,
                Username = user.Username,
                PPUrl = profile.PPUrl
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto, string ipAddress)
        {
            var user = await _usersRepository.GetByUsernameAsync(dto.Username);
            if (user == null)
                return null;

            bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
            if (!passwordValid)
            {
                await _usersRepository.IncrementFailedLoginAsync(user.UserID);
                return null;
            }

            await _usersRepository.UpdateLoginSuccessAsync(user.UserID, ipAddress, DateTime.UtcNow);

            string token = _tokenService.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = new UserSummaryDto
                {
                    UserID = user.UserID,
                    Username = user.Username,
                    PPUrl = null
                }
            };
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.UtcNow;
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
                age--;
            return age;
        }
    }
}
