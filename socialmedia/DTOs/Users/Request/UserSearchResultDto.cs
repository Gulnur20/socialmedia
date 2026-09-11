namespace socialmedia.DTOs.Users.Request
{
    public class UserSearchResultDto
    {
        public long UserID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PPUrl { get; set; }
    }
}
