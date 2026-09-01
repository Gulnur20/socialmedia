namespace socialmedia.Models
{
    public class CommentLike
    {
        public long CommentLikeID { get; set; }
        public long CommentID { get; set; }
        public long UserID { get; set; }
        public DateTime CommentLikedDate { get; set; }
        public Comment? Comment { get; set; }

    }
}
