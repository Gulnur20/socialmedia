using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using socialmedia.DTOs.Users.Request;
using socialmedia.DTOs.Users.Response;
using socialmedia.Services.Interfaces;
using System.Security.Claims;

namespace socialmedia.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<MyProfileDto>> GetMyProfile()
        {
            long userId = GetCurrentUserId();

            var result = await _userService.GetMyProfileAsync(userId);
            return Ok(result);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(long id)
        {
            long currentUserId = User.Identity?.IsAuthenticated == true
              ? GetCurrentUserId()
              : 0;

            var result = await _userService.GetUserProfileAsync(id, currentUserId);
            return Ok(result);
        }
        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            long userId = GetCurrentUserId();
            await _userService.UpdateUserProfileAsync(userId, dto);

            return NoContent();
        }
        [Authorize]
        [HttpPut("settings")]
        public async Task<ActionResult> UpdateSettings(UpdateSettingsDto dto)
        {
            long userId = GetCurrentUserId();
            await _userService.UpdateUserSettingsAsync(userId, dto);
            return NoContent();
        }

        [Authorize]
        [HttpPost("freeze")]
        public async Task<ActionResult> FreezeAccount()
        {
            long userId = GetCurrentUserId();
            await _userService.FreezeAccountAsync(userId);
            return NoContent();
        }

        [Authorize]
        [HttpPost("activate")]
        public async Task<ActionResult> ActivateAccount()
        {
            long userId = GetCurrentUserId();
            await _userService.ActivateAccountAsync(userId);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("me")]
        public async Task<ActionResult> DeleteAccount()
        {
            long userId = GetCurrentUserId();
            await _userService.DeleteAccountAsync(userId);
            return NoContent();
        }

        private long GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            return long.Parse(claim);
        }




    }
}
