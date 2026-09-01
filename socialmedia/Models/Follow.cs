namespace socialmedia.Models
{
    public class Follow
    {
        public long FollowID { get; set; }
        public long FollowerID { get; set; }
        public long FollowingID  { get; set; }
         public DateTime FollowDate { get; set; }
    }
}
