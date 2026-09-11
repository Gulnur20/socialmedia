using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace socialmedia.Controllers 
{


    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public UploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UploadFile(IFormFile file, [FromQuery] string uploadType = "post")
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Dosya seçilmedi." });

            var allowedExtensions = uploadType == "profile"
                ? new[] { ".jpg", ".jpeg", ".png", ".webp" }
                : new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest(new
                {
                    message = uploadType == "profile"
                    ? "Profil fotoğrafı için sadece JPG, PNG veya WEBP kullanılabilir."
                    : "Sadece resim dosyaları yüklenebilir."
                });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "Dosya boyutu 5MB'ı geçemez." });

            bool isValidImage = await IsValidImageContentAsync(file, uploadType);
            if (!isValidImage)
                return BadRequest(new { message = "Dosya içeriği geçerli bir resim değil." });

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";

            return Ok(new { url = fileUrl });
        }

        private async Task<bool> IsValidImageContentAsync(IFormFile file, string uploadType)
        {
            byte[] header = new byte[8];

            using (var stream = file.OpenReadStream())
            {
                await stream.ReadAsync(header, 0, header.Length);
            }

            bool isJpeg = header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
            bool isPng = header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47;
            bool isGif = header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38;
            bool isWebp = header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46;

            if (uploadType == "profile")
                return isJpeg || isPng || isWebp;

            return isJpeg || isPng || isGif || isWebp;
        }
    }
}