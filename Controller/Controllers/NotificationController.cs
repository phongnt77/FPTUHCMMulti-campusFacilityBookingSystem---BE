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
    /// API quản lý thông báo (Notifications)
    /// </summary>
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Lấy danh sách thông báo với filtering
        /// </summary>
        /// <param name="filter">Bộ lọc (userId, type, status, page, limit)</param>
        /// <returns>Danh sách thông báo</returns>
        /// <response code="200">Trả về danh sách thành công</response>
        /// <response code="401">Chưa đăng nhập</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithPagination<List<NotificationResponseDto>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetAll([FromQuery] NotificationFilterDto filter)
        {
            try
            {
                // Nếu không có userId trong filter, lấy từ token
                if (string.IsNullOrEmpty(filter.UserId))
                {
                    var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                                User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        filter.UserId = userId;
                    }
                }

                var result = await _notificationService.GetFilteredAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        /// <summary>
        /// Lấy tất cả thông báo chưa đọc của user hiện tại
        /// </summary>
        /// <returns>Danh sách thông báo chưa đọc</returns>
        /// <response code="200">Trả về danh sách thành công</response>
        /// <response code="401">Chưa đăng nhập</response>
        [HttpGet("unread")]
        [ProducesResponseType(typeof(ApiResponse<List<NotificationResponseDto>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetUnread()
        {
            try
            {
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                            User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse.Fail(401, "Không tìm thấy user id trong token."));
                }

                var result = await _notificationService.GetUnreadByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        /// <summary>
        /// Lấy số lượng thông báo chưa đọc của user hiện tại
        /// </summary>
        /// <returns>Số lượng thông báo chưa đọc</returns>
        /// <response code="200">Trả về số lượng thành công</response>
        /// <response code="401">Chưa đăng nhập</response>
        [HttpGet("unread/count")]
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                            User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse.Fail(401, "Không tìm thấy user id trong token."));
                }

                var result = await _notificationService.GetUnreadCountByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        /// <summary>
        /// Đánh dấu thông báo là đã đọc
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <returns>Kết quả</returns>
        /// <response code="200">Đánh dấu thành công</response>
        /// <response code="404">Không tìm thấy thông báo</response>
        [HttpPut("{id}/read")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            try
            {
                var result = await _notificationService.MarkAsReadAsync(id);
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
        /// Đánh dấu tất cả thông báo của user hiện tại là đã đọc
        /// </summary>
        /// <returns>Kết quả</returns>
        /// <response code="200">Đánh dấu thành công</response>
        /// <response code="401">Chưa đăng nhập</response>
        [HttpPut("read-all")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                            User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse.Fail(401, "Không tìm thấy user id trong token."));
                }

                var result = await _notificationService.MarkAllAsReadByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        /// <summary>
        /// Xóa thông báo
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <returns>Kết quả</returns>
        /// <response code="200">Xóa thành công</response>
        /// <response code="404">Không tìm thấy thông báo</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _notificationService.DeleteAsync(id);
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

