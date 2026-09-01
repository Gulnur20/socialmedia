using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using socialmedia.DTOs.Common;
using socialmedia.DTOs.Post.Request;
using socialmedia.DTOs.Post.Response;
using socialmedia.Services.Interfaces;
using System.Security.Claims;


namespace socialmedia.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PostController:ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

      
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PostDto>> CreatePost(CreatePostDto dto)
        {
            long userId = GetCurrentUserId();
            var result = await _postService.CreatePostAsync(dto, userId);
            return CreatedAtAction(nameof(GetPostById), new { id = result.PostID }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPostById(long id)
        {
            long currentUserId = User.Identity?.IsAuthenticated == true
                ? GetCurrentUserId()
                : 0;

            var result = await _postService.GetPostByIdAsync(id, currentUserId);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<PostDto>> UpdatePost(long id, UpdatePostDto dto)
        {
            long userId = GetCurrentUserId();
            var result = await _postService.UpdatePostAsync(id, dto, userId);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePost(long id)
        {
            long userId = GetCurrentUserId();
            await _postService.DeletePostAsync(id, userId);
            return NoContent();
        }

        [Authorize]
        [HttpPost("{id}/like")]
        public async Task<ActionResult> ToggleLike(long id)
        {
            long userId = GetCurrentUserId();
            bool isNowLiked = await _postService.TogglePostLikeAsync(id, userId);

   
            return Ok(new { isLiked = isNowLiked });
        }

        [Authorize]
        [HttpGet("timeline")]
        public async Task<ActionResult<PagedResultDto<PostDto>>> GetTimeline(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            long userId = GetCurrentUserId();
            var result = await _postService.GetTimelineAsync(userId, page, pageSize);
            return Ok(result);
        }

        private long GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claim == null)
                throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");
            return long.Parse(claim);
        }
    }
}
