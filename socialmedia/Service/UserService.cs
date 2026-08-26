using Microsoft.Data.SqlClient;
using socialmedia.Models;
using socialmedia.Repositories.UserRepostories;

namespace socialmedia.Service
{
    public class UserService
    {
        private readonly UsersRepository _usersRepository;
        public UserService(UsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        public async Task   AddNewUserAsync(string username, string password, string email, string firstName, string lastName, DateTime birthDate)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Kullanıcı adı boş bırakılamaz.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Şifre boş bırakılamaz.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("e-posta alanı boş bırakılamaz.");
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("Ad alanı boş bırakılamaz.");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Soyad alanı boş bırakılamaz.");
            }

            if (birthDate == DateTime.MinValue)
            {
                throw new ArgumentException("Doğum tarihi alanı boş bırakılamaz.");
            }

            int age = DateTime.Now.Year - birthDate.Year;
            if (birthDate.Date > DateTime.Now.AddYears(-age))
            {
                age--;
            }

            if (age < 15)
            {
                throw new ArgumentException("Kayıt olabilmek için en az 15 yaşında olmalısınız.");
            }

            bool IsUsernameTaken = await _usersRepository.CheckUsernameAsync(username);

            if (IsUsernameTaken)
            {
                throw new ArgumentException("Bu kullanıcı adı zaten kullanılıyor. Lütfen başka bir kullanıcı adı seçin.");
            }

            Users user = new Users
            {
                UserID = 0, 
                Username = username,
                Password = password, 
                Email = email,
                IsActive = true,
                UserCreated = DateTime.Now
            };

            UserProfile profile = new UserProfile
            {
                FirstName = firstName,
                LastName = lastName,
                Biography = null,
                PPUrl = null,
                BirthDate = birthDate
            };

            UserSettings settings = new UserSettings
            {
                IsPrivate = false,
                IsVerified = false,
                IsEmailConfirmed = false,
                FailedLoginCount = 0,
                LastUsernameChanged = DateTime.Now,
                LastPasswordChanged = DateTime.Now,
                LastLoginIP = "127.0.0.1",
                LastLoginDate = DateTime.Now,
                DeletedDate = default
            };

            await _usersRepository.AddUserAsync(user, profile, settings);
        }
        public async Task UpdateUserProfileAsync(int userId, string firstName, string lastName, string biography, string ppUrl)
        {
         
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("Ad alanı boş bırakılamaz.");
            }

            if ( string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Soyad alanı boş bırakılamaz.");
            }

            await _usersRepository.UpdateProfileAsync(userId, firstName, lastName, biography, ppUrl);
        }

        public async Task FreezeAccountAsync(int userId)
        {
            await _usersRepository.UpdateStatusAsync(userId, false);
        }
        public async Task ActivateAccountAsync(int userId)
        {
            await _usersRepository.UpdateStatusAsync(userId, true);
        }
        public async Task DeleteAccountAsync(int userId)
        {
            DateTime deletedDate = DateTime.Now;
            await _usersRepository.DeleteUserAsync(userId, deletedDate);
        }
    }
}
