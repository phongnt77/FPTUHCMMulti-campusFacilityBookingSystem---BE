using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Controller.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                         User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse.Fail(401, "User id is missing in token."));
            }

            var result = await _userService.GetProfileAsync(userId);

            if (result == null)
            {
                return NotFound(ApiResponse.Fail(404, "User not found."));
            }

            return Ok(ApiResponse<UserResponseDto>.Ok(result));
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail(400, "Invalid request data"));
            }

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                         User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse.Fail(401, "User id is missing in token."));
            }

            var result = await _userService.UpdateProfileAsync(userId, dto);

            if (result == null)
            {
                return NotFound(ApiResponse.Fail(404, "User not found."));
            }

            return Ok(ApiResponse<UserResponseDto>.Ok(result));
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail(400, "Invalid request data"));
            }

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                         User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse.Fail(401, "User id is missing in token."));
            }

            try
            {
                var result = await _userService.ChangePasswordAsync(userId, dto);
                if (result)
                {
                    return Ok(ApiResponse.Ok());
                }
                return BadRequest(ApiResponse.Fail(400, "Failed to change password."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.Fail(400, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.Fail(400, ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse.Fail(401, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        [HttpGet]
        [Authorize(Roles = "RL0003")]
        public async Task<IActionResult> GetAll([FromQuery] UserFilterDto filter)
        {
            try
            {
                var result = await _userService.GetAllAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "RL0003")] 
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var result = await _userService.GetByIdAsync(id);
                if (!result.Success)
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }
    }
}



