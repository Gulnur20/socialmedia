namespace socialmedia.Models
{
    public class UserProfile
    {
        public long UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Biography { get; set; }
        public string? PPUrl { get; set; }
        public DateTime BirthDate { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }


    }
}
