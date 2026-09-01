namespace socialmedia.Models
{
    public class PostLike
    {
        public long PostLikeID { get; set; }
        public long PostID { get; set; }
        public long UserID { get; set; }
        public DateTime PostLikedDate { get; set; }
        public MainPost? MainPost { get; set; }

    }
}
