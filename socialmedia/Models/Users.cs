namespace socialmedia.Models
{
    public class Users
    {
        public required long UserID { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required bool IsActive {  get; set; }
        public required DateTime UserCreated { get; set; }
        
    }
}
