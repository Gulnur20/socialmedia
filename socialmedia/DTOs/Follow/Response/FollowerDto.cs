using socialmedia.DTOs.Users.Response;

namespace socialmedia.DTOs.Follow.Response
{
    public class FollowerDto
    {
        public int FollowID { get; set; }
        public UserSummaryDto User { get; set; }
        public DateTime FollowDate { get; set; }
    }
}
