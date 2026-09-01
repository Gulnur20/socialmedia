using socialmedia.DTOs.Users.Response;


    namespace socialmedia.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public UserSummaryDto User { get; set; }
    }

}
