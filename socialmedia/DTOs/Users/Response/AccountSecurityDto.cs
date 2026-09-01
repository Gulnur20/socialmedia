namespace socialmedia.DTOs.Users.Response
{
    public class AccountSecurityDto
    {
        public DateTime LastLoginDate { get; set; }
        public string LastLoginIP { get; set; }
        public DateTime LastPasswordChanged { get; set; }
        public DateTime LastUsernameChanged { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
}
