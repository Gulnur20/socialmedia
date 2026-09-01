using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using socialmedia.DTOs.Comment.Request;
using socialmedia.DTOs.Comment.Response;
using socialmedia.Services.Interfaces;
using System.Security.Claims;

namespace socialmedia.Controllers
{
    [ApiController]
   
    [Route("api/[controller]")]
    public class CommentController:ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [Authorize]
        [HttpPost("{postId}")]
        public async Task<ActionResult<CommentDto>> AddComment(long postId, [FromBody] CreateCommentDto dto)
        {
            long userId = GetCurrentUserId();
            var result = await _commentService.AddCommentAsync(postId, dto, userId);

            return Ok(result);
        }

        [HttpGet("post/{postId}")]
        public async Task<ActionResult<List<CommentDto>>> GetCommentsByPostId(long postId)
        {
            long currentUserId = User.Identity?.IsAuthenticated == true ? GetCurrentUserId() : 0;

            var result = await _commentService.GetCommentsByPostIdAsync(postId, currentUserId);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<CommentDto>> UpdateComment(long id, [FromBody] UpdateCommentDto dto)
        {
            long userId = GetCurrentUserId();
            var result = await _commentService.UpdateCommentAsync(id, dto, userId);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(long id)
        {
            long userId = GetCurrentUserId();
            await _commentService.DeleteCommentAsync(id, userId);

            return NoContent();
        }

   
        [Authorize]
        [HttpPost("{id}/like")]
        public async Task<ActionResult> ToggleLike(long id)
        {
            long userId = GetCurrentUserId();
            bool isNowLiked = await _commentService.ToggleCommentLikeAsync(id, userId);

            return Ok(new { isLiked = isNowLiked });
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
