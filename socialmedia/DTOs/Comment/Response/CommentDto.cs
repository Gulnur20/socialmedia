using socialmedia.DTOs.Users.Response;

namespace socialmedia.DTOs.Comment.Response
{
    public class CommentDto
    {
        public long CommentID { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; }
        public bool IsUpdate { get; set; }
        public DateTime? UpdateTime { get; set; }
        public UserSummaryDto Author { get; set; }
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public long? ParentCommentID { get; set; }
        public List<CommentDto> Replies { get; set; }
    }
}
