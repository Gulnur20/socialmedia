namespace socialmedia.Models
{
    public class PostsLike
    {
        public int PostLikeID { get; set; }
        public int PostID { get; set; }
        public int UserID { get; set; }
        public DateTime PostLikedDate { get; set; }

    }
}
