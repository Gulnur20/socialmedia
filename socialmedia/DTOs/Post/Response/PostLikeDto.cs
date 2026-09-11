using socialmedia.DTOs.Users.Response;

namespace socialmedia.DTOs.Post.Response
{
    public class PostLikeDto
    {
        
            public UserSummaryDto User { get; set; }
            public DateTime PostLikedDate { get; set; }
        
    }
}
