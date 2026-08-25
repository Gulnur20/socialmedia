namespace socialmedia.DataTransferObject
{
    public class UserListDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PPUrl { get; set; }
        public bool IsVerified { get; set; }
        public bool IsFollowing { get; set; } 
        public bool IsRequestSent { get; set; }
    }
}
