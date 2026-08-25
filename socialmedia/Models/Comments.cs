using System.Data;

namespace socialmedia.Models
{
    public class Comments
    {
        public int CommentID { get; set; }
        public int PostID { get; set; }
        public int UserID  { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate  { get; set; }
        public bool IsUpdate { get; set; }
        public DateTime UpdateTime { get; set; }
        public int LikeCount    { get; set; }
        public int ParentCommentID { get; set; }

    }
}
