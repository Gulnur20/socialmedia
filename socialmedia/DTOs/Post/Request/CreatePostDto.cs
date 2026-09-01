using System.ComponentModel.DataAnnotations;

namespace socialmedia.DTOs.Post.Request
{
    public class CreatePostMediaItemDto
    {
        [Required]
        public string Url { get; set; }

        // Client "Image" ya da "Video" gönderecek (büyük/küçük harf duyarsız).
        [Required]
        public string MediaType { get; set; }
    }

    public class CreatePostDto
    {
        public string? Caption { get; set; }

        [Required(ErrorMessage = "En az bir medya eklemelisiniz.")]
        [MinLength(1, ErrorMessage = "En az bir medya eklemelisiniz.")]
        [MaxLength(10, ErrorMessage = "Bir postta en fazla 10 medya olabilir.")]
        public List<CreatePostMediaItemDto> Media { get; set; }
    }
}
