namespace socialmedia.DataTransferObject
{
    public class FeedDTO
    {
        public int PostID { get; set; }
        public string Caption { get; set; }
        public DateTime PostCreated { get; set; }
        public bool IsEdited { get; set; }
        public int CommentCount { get; set; }
        public int LikeCount { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PPUrl { get; set; }
        public bool IsVerified { get; set; }
        public List<MediaDTO> MediaList { get; set; } = new List<MediaDTO>();
        public bool IsLikedByCurrentUser { get; set; }
    }
}
