using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail(400, "Dữ liệu không hợp lệ."));
            }

            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(ApiResponse<AuthResponseDto>.Ok(result));
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

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // gọi POST /api/auth/logout nếu 200 OK -> FE xóa token khỏi storage -> trả về trang login
            return Ok(ApiResponse.Ok());
        }

        [HttpPost("login/google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail(400, "Dữ liệu không hợp lệ."));
            }

            try
            {
                var result = await _authService.LoginWithGoogleAsync(dto);
                return Ok(ApiResponse<AuthResponseDto>.Ok(result));
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

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string code)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                return BadRequest(ApiResponse.Fail(400, "Email và mã xác thực là bắt buộc."));
            }

            try
            {
                var result = await _authService.VerifyEmailAsync(email, code);
                if (result)
                {
                    return Ok(ApiResponse.Ok());
                }
                return BadRequest(ApiResponse.Fail(400, "Mã xác thực không hợp lệ hoặc đã hết hạn."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        [HttpPost("resend-verification-email")]
        public async Task<IActionResult> ResendVerificationEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(ApiResponse.Fail(400, "Email là bắt buộc."));
            }

            try
            {
                var result = await _authService.ResendVerificationEmailAsync(email);
                if (result)
                {
                    return Ok(ApiResponse.Ok());
                }
                return BadRequest(ApiResponse.Fail(400, "Không thể gửi email xác thực."));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResponse.Fail(404, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.Fail(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(ApiResponse.Fail(400, "Email là bắt buộc."));
            }

            try
            {
                var result = await _authService.ForgotPasswordAsync(email);
                if (result)
                {
                    return Ok(ApiResponse.Ok());
                }
                return BadRequest(ApiResponse.Fail(400, "Không thể gửi mã đặt lại mật khẩu."));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResponse.Fail(404, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.Fail(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromQuery] string email, [FromQuery] string code, [FromQuery] string newPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest(ApiResponse.Fail(400, "Email, mã xác thực và mật khẩu mới là bắt buộc."));
            }

            try
            {
                var result = await _authService.ResetPasswordAsync(email, code, newPassword);
                if (result)
                {
                    return Ok(ApiResponse.Ok());
                }
                return BadRequest(ApiResponse.Fail(400, "Mã đặt lại mật khẩu không hợp lệ hoặc đã hết hạn."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }
    }
}

