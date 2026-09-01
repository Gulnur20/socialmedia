using System.Data;

namespace socialmedia.Models
{
    public class Comment
    {
        public long CommentID { get; set; }
        public long PostID { get; set; }
        public long UserID  { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate  { get; set; }
        public bool IsUpdate { get; set; }
        public DateTime UpdateTime { get; set; }
        public int LikeCount    { get; set; }
        public long? ParentCommentID { get; set; }
        public List<CommentLike> CommentLikes { get; set; } = new();
        public List<Comment> Replies { get; set; } = new();
        public MainPost? MainPost { get; set; }
        public Users? User { get; set; }

    }
}
