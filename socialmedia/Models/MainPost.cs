namespace socialmedia.Models
{
    public class MainPost
    {   public long PostID { get; set; }
        public long UserID { get; set; }
        public string? Caption { get; set; }
        public int LikeCount   { get; set; }
        public int CommentCount { get; set; }
        public bool IsEdited    { get; set; }
        public DateTime PostCreated { get; set; }
        public List<PostMedia> PostMedia { get; set; } = new();
        public List<PostLike> PostLikes { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public Users? User { get; set; }

    }
}
