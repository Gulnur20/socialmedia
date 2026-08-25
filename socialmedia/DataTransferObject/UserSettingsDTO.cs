namespace socialmedia.DataTransferObject
{
    public class UserSettingsDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsVerified { get; set; }
        public bool IsEmailConfirmed { get; set; } 
        public DateTime LastUsernameChanged { get; set; }
        public DateTime LastPasswordChanged { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}
