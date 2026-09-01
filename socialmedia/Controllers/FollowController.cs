using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using socialmedia.DTOs.Follow.Response;
using socialmedia.Services.Interfaces;
using System.Security.Claims;
namespace socialmedia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FollowController:ControllerBase
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        [Authorize]
        [HttpPost("{followingId}")]
        public async Task<ActionResult<FollowActionResultDto>> FollowUser(long followingId)
        {
            long followerId = GetCurrentUserId();
            var result = await _followService.FollowUserAsync(followerId, followingId);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{followingId}")]
        public async Task<ActionResult> UnfollowUser(long followingId)
        {
            long followerId = GetCurrentUserId();
            await _followService.UnfollowAsync(followerId, followingId);
            return NoContent();
        }

        [Authorize]
        [HttpGet("requests")]
        public async Task<ActionResult<List<FollowRequestDto>>> GetWaitingFollowRequests()
        {
            long userId = GetCurrentUserId();
            var result = await _followService.GetWaitingFollowRequestsAsync(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("requests/{requestId}/accept")]
        public async Task<ActionResult> AcceptFollowRequest(long requestId)
        {
            await _followService.AcceptFollowRequestAsync(requestId);
            return NoContent();
        }

        [Authorize]
        [HttpPost("requests/{requestId}/reject")]
        public async Task<ActionResult> RejectFollowRequest(long requestId)
        {
            await _followService.RejectFollowRequestAsync(requestId);
            return NoContent();
        }

        private long GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            return long.Parse(claim);
        }
    }
}
