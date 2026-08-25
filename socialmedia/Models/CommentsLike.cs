namespace socialmedia.Models
{
    public class CommentsLike
    {
        public int CommentLikeID { get; set; }
        public int CommentID { get; set; }
        public int UserID { get; set; }
        public DateTime CommentLikedDate { get; set; }

    }
}
