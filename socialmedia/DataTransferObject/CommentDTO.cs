namespace socialmedia.DataTransferObject
{
    public class CommentDTO
    {
        public int CommentID { get; set; }
        public int PostID { get; set; }
        public int ParentCommentID { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; }
        public bool IsUpdate { get; set; }
        public int LikeCount { get; set; } 
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PPUrl { get; set; }
        public bool IsVerified { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }
}
