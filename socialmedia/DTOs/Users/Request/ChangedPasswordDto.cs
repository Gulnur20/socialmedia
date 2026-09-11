using System.ComponentModel.DataAnnotations;

namespace socialmedia.DTOs.Users.Request
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.")]
        public string NewPassword { get; set; }
    }
}