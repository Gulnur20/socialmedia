namespace socialmedia.DataTransferObject
{
    public class FollowRequestDTO
    {
        public int RequestID { get; set; }
        public DateTime RequestDate { get; set; }
        public int FollowerID { get; set; } 
        public string FollowerUsername { get; set; }
        public string FollowerFirstName { get; set; }
        public string FollowerLastName { get; set; }
        public string FollowerPPUrl { get; set; } 
        public bool IsVerified { get; set; }
    }
}
