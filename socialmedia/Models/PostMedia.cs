namespace socialmedia.Models
{
    public class PostMedia
    {
        public int MediaID { get; set; }
        public int PostID { get; set; }
        public string MediaURL { get; set; }
        public string MediaType { get; set; }
        public string DisplayOrder { get; set; }

    }
}
