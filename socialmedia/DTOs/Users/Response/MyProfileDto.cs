namespace socialmedia.DTOs.Users.Response
{
    public class MyProfileDto
    {
        public long UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Biography { get; set; }
        public string PPUrl { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsVerified { get; set; }
        public bool IsPrivate { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public int PostCount { get; set; }
    }
}
