namespace socialmedia.DTOs.Users.Request
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Biography { get; set; }
        public string PPUrl { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
