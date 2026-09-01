namespace socialmedia.Models
{
    public enum MediaType : byte
    {
        Image = 1,
        Video = 2
    }

    public class PostMedia
    {
        public long MediaID { get; set; }
        public long PostID { get; set; }
        public string MediaURL { get; set; }
        public  MediaType MediaType { get; set;}
        public byte DisplayOrder { get; set; }
        public MainPost? MainPost { get; set; }

    }
}
