namespace socialmedia.Models
{
    public class UserSettings
    {
        public long UserID { get; set; }
        public bool IsPrivate { get; set; } 
        public bool IsVerified { get; set; }    
        public bool IsEmailConfirmed { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime LastUsernameChanged { get; set; }
        public DateTime LastPasswordChanged { get; set; }
        public string? LastLoginIP { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
