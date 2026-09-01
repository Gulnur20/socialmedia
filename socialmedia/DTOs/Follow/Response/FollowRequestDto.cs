using socialmedia.DTOs.Users.Response;

namespace socialmedia.DTOs.Follow.Response
{
    public class FollowRequestDto
    {
        public long RequestID { get; set; }
        public UserSummaryDto RequestingUser { get; set; }
        public DateTime RequestedDate { get; set; }
    }

}
