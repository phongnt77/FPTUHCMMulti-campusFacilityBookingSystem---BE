using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Controller.Controllers
{
    /// <summary>
    /// API quản lý thông tin user
    /// </summary>
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

        /// <summary>
        /// Lấy thông tin profile của user hiện tại
        /// </summary>
        /// <returns>Thông tin user</returns>
        /// <response code="200">Trả về thông tin thành công</response>
        /// <response code="401">Chưa đăng nhập hoặc token không hợp lệ</response>
        /// <response code="404">Không tìm thấy user</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Xem thông tin cá nhân (profile page)
        /// 
        /// User ID lấy từ JWT token
        /// </remarks>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                         User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse.Fail(401, "Không tìm thấy user id trong token."));
            }

            var result = await _userService.GetProfileAsync(userId);

            if (result == null)
            {
                return NotFound(ApiResponse.Fail(404, "Không tìm thấy user."));
            }

            return Ok(ApiResponse<UserResponseDto>.Ok(result));
        }

        /// <summary>
        /// Cập nhật thông tin profile
        /// </summary>
        /// <param name="dto">Thông tin cần cập nhật</param>
        /// <returns>Thông tin user sau khi cập nhật</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="401">Chưa đăng nhập</response>
        /// <response code="404">Không tìm thấy user</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Cập nhật thông tin cá nhân (FullName, PhoneNumber, AvatarUrl)
        /// 
        /// User chỉ có thể cập nhật profile của chính mình
        /// </remarks>
        [HttpPut("profile")]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail(400, "Dữ liệu không hợp lệ"));
            }

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                         User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse.Fail(401, "Không tìm thấy user id trong token."));
            }

            var result = await _userService.UpdateProfileAsync(userId, dto);

            if (result == null)
            {
                return NotFound(ApiResponse.Fail(404, "Không tìm thấy user."));
            }

            return Ok(ApiResponse<UserResponseDto>.Ok(result));
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="dto">Mật khẩu cũ và mới</param>
        /// <returns>Kết quả đổi mật khẩu</returns>
        /// <response code="200">Đổi mật khẩu thành công</response>
        /// <response code="400">Mật khẩu cũ không đúng hoặc dữ liệu không hợp lệ</response>
        /// <response code="401">Chưa đăng nhập</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Đổi mật khẩu (yêu cầu nhập mật khẩu cũ)
        /// 
        /// **Lưu ý:** Chỉ áp dụng cho user đăng nhập bằng email/password (không áp dụng cho Google login)
        /// </remarks>
        [HttpPut("change-password")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail(400, "Dữ liệu không hợp lệ"));
            }

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                         User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse.Fail(401, "Không tìm thấy user id trong token."));
            }

            try
            {
                var result = await _userService.ChangePasswordAsync(userId, dto);
                if (result)
                {
                    return Ok(ApiResponse.Ok());
                }
                return BadRequest(ApiResponse.Fail(400, "Đổi mật khẩu thất bại."));
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

        /// <summary>
        /// Lấy danh sách tất cả users với filtering (Admin only)
        /// </summary>
        /// <param name="filter">Bộ lọc và pagination</param>
        /// <returns>Danh sách users</returns>
        /// <response code="200">Trả về danh sách thành công</response>
        /// <response code="403">Không có quyền</response>
        /// <remarks>
        /// **Roles:** Chỉ Facility_Admin (RL0003)
        /// 
        /// **Mục đích:** Quản lý danh sách users trong hệ thống
        /// 
        /// **Filters:**
        /// - `name`: Tìm theo tên (contains)
        /// - `email`: Tìm theo email (contains)
        /// - `roleId`: Lọc theo role (RL0001, RL0002, RL0003)
        /// - `campusId`: Lọc theo campus
        /// - `status`: Active | Inactive
        /// - `page`: Trang (default: 1)
        /// - `limit`: Số items/trang (default: 10)
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "RL0003")]
        [ProducesResponseType(typeof(ApiResponseWithPagination<List<UserResponseDto>>), 200)]
        [ProducesResponseType(403)]
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

        /// <summary>
        /// Lấy chi tiết user (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Thông tin user</returns>
        /// <response code="200">Trả về thông tin thành công</response>
        /// <response code="403">Không có quyền</response>
        /// <response code="404">Không tìm thấy user</response>
        /// <remarks>
        /// **Roles:** Chỉ Facility_Admin (RL0003)
        /// 
        /// **Mục đích:** Xem chi tiết thông tin user khác
        /// </remarks>
        [HttpGet("{id}")]
        [Authorize(Roles = "RL0003")] 
        [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
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

        /// <summary>
        /// Xóa user (soft delete - set status = Inactive)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Kết quả xóa</returns>
        /// <response code="200">Xóa thành công</response>
        /// <response code="403">Không có quyền</response>
        /// <response code="404">Không tìm thấy user</response>
        /// <remarks>
        /// **Roles:** Chỉ Facility_Admin (RL0003)
        /// 
        /// **Mục đích:** Vô hiệu hóa user account (soft delete)
        /// 
        /// **Lưu ý:** 
        /// - Không xóa hẳn khỏi database
        /// - Chỉ set status = Inactive
        /// - User không thể đăng nhập sau khi bị xóa
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize(Roles = "RL0003")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _userService.DeleteAsync(id);
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
