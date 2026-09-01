using socialmedia.DTOs.Users.Response;

namespace socialmedia.DTOs.Post.Response
{
    public class PostDto
    {
        public long PostID { get; set; }
        public string? Caption { get; set; }
        public DateTime PostCreated { get; set; }
        public bool IsEdited { get; set; }
        public UserSummaryDto Author { get; set; }
        public List<PostMediaDto> Media { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }

}
