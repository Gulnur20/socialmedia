namespace socialmedia.Models
{
    public class MainPost
    {   public int PostID { get; set; }
        public int UserID { get; set; }
        public string Caption { get; set; }
        public int LikeCount   { get; set; }
        public int CommentCount { get; set; }
        public bool IsEdited    { get; set; }
        public DateTime PostCreated { get; set; }

    }
}
