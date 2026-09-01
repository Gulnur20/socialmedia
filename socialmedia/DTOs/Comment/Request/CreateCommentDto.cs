namespace socialmedia.DTOs.Comment.Request
{
    public class CreateCommentDto
    {
        public string CommentText { get; set; }
        public long? ParentCommentID { get; set; }
    }
}
