namespace socialmedia.Models
{
    public class Follows
    {
        public int FollowID { get; set; }
        public int FollowerID { get; set; }
        public int FollowingID  { get; set; }
         public DateTime FollowDate { get; set; }
    }
}
