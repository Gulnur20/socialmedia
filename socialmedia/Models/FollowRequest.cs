namespace socialmedia.Models
{
    public class FollowRequest
    {
        public int RequestID { get; set; }
        public int FollowerID { get; set; }
        public int FollowingID  { get; set; }
        public DateTime RequestDate { get; set; }

    }
}
