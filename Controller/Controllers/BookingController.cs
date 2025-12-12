using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using DAL.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Controller.Controllers
{
    /// <summary>
    /// API quản lý đặt phòng/cơ sở (Bookings)
    /// </summary>
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Lấy danh sách bookings với filtering
        /// </summary>
        /// <param name="filter">Bộ lọc (userId, facilityId, status, page, limit)</param>
        /// <returns>Danh sách bookings</returns>
        /// <response code="200">Trả về danh sách thành công</response>
        /// <response code="401">Chưa đăng nhập</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập (Student, Lecturer, Admin)
        /// 
        /// **Filters:**
        /// - userId: Lọc theo người đặt
        /// - facilityId: Lọc theo cơ sở
        /// - status: Draft | Pending_Approval | Approved | Rejected | Cancelled | Completed | No_Show
        /// - page: Trang (default: 1)
        /// - limit: Số items/trang (default: 10)
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithPagination<List<BookingResponseDto>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetAll([FromQuery] BookingFilterDto filter)
        {
            try
            {
                var result = await _bookingService.GetAllAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        /// <summary>
        /// Lấy tất cả bookings của user hiện tại
        /// </summary>
        /// <param name="status">Filter theo status (Draft, Pending_Approval, Approved, Rejected, Cancelled, Completed, No_Show)</param>
        /// <param name="page">Trang hiện tại (default: 1)</param>
        /// <param name="limit">Số item per page (default: 10)</param>
        /// <returns>Danh sách bookings của user</returns>
        /// <response code="200">Trả về danh sách bookings</response>
        /// <response code="401">Không xác thực được user</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** User xem lại tất cả bookings của mình với filter theo status và phân trang
        /// 
        /// **Status Values:**
        /// - Draft: Booking đang soạn thảo
        /// - Pending_Approval: Đang chờ duyệt
        /// - Approved: Đã được duyệt
        /// - Rejected: Bị từ chối
        /// - Cancelled: Đã hủy
        /// - Completed: Đã hoàn thành
        /// - No_Show: User không tới
        /// </remarks>
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponseWithPagination<List<BookingResponseDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        public async Task<IActionResult> GetMyBookings([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            try
            {
                // Lấy userId từ JWT token
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                            User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse.Fail(401, "Không tìm thấy user id trong token."));
                }

                // Parse status string to enum
                BookingStatus? statusEnum = null;
                if (!string.IsNullOrEmpty(status))
                {
                    if (Enum.TryParse<BookingStatus>(status, true, out var parsedStatus))
                    {
                        statusEnum = parsedStatus;
                    }
                }

                // Tạo filter với userId từ token
                var filter = new BookingFilterDto
                {
                    UserId = userId,
                    Status = statusEnum,
                    Page = page,
                    Limit = limit
                };

                var result = await _bookingService.GetAllAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        /// <summary>
        /// Lấy chi tiết booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Thông tin chi tiết booking</returns>
        /// <response code="200">Trả về thông tin thành công</response>
        /// <response code="404">Không tìm thấy booking</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// </remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BookingResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var result = await _bookingService.GetByIdAsync(id);
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
        /// Tạo booking mới (tự động kiểm tra conflict thời gian)
        /// </summary>
        /// <param name="dto">Thông tin booking</param>
        /// <returns>Booking đã tạo</returns>
        /// <response code="200">Tạo thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="409">Khung giờ đã được đặt trước</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Tạo lượt đặt cơ sở vật chất mới
        /// 
        /// Hệ thống tự động:
        /// - Kiểm tra conflict thời gian
        /// - Tạo booking với status = Pending_Approval (chờ admin duyệt)
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<BookingResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail(400, "Dữ liệu không hợp lệ."));
            }

            try
            {
                var result = await _bookingService.CreateAsync(dto);
                if (!result.Success)
                {
                    return Conflict(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        /// <summary>
        /// Cập nhật thông tin booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="dto">Thông tin cập nhật</param>
        /// <returns>Booking đã cập nhật</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="404">Không tìm thấy booking</response>
        /// <response code="409">Khung giờ mới bị conflict</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Cập nhật thông tin booking (thời gian, mục đích, trạng thái, etc.)
        /// 
        /// Nếu thay đổi thời gian, hệ thống tự động kiểm tra conflict
        /// </remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BookingResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 409)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateBookingDto dto)
        {
            try
            {
                var result = await _bookingService.UpdateAsync(id, dto);
                if (!result.Success)
                {
                    if (result.Error?.Code == 409)
                        return Conflict(result);
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
        /// Submit booking để chuyển từ Draft → Pending_Approval
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking đã submit</returns>
        /// <response code="200">Submit thành công</response>
        /// <response code="400">Booking không ở trạng thái Draft</response>
        /// <response code="404">Không tìm thấy booking</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Submit booking để gửi yêu cầu duyệt
        /// 
        /// **Workflow:**
        /// - Tạo booking → Status = Draft
        /// - Submit booking → Status = Pending_Approval (chờ admin duyệt)
        /// - Admin approve/reject → Status = Approved/Rejected
        /// 
        /// **Lưu ý:** Chỉ có thể submit booking ở trạng thái Draft
        /// </remarks>
        [HttpPost("{id}/submit")]
        [ProducesResponseType(typeof(ApiResponse<BookingResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> Submit(string id)
        {
            try
            {
                var result = await _bookingService.SubmitBookingAsync(id);
                if (!result.Success)
                {
                    if (result.Error?.Code == 400)
                        return BadRequest(result);
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
        /// Hủy booking (set status = Cancelled)
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="reason">Lý do hủy (optional)</param>
        /// <returns>Kết quả hủy</returns>
        /// <response code="200">Hủy thành công</response>
        /// <response code="400">Không thể hủy (đã quá thời gian cho phép hoặc trạng thái không hợp lệ)</response>
        /// <response code="403">Không có quyền hủy booking này</response>
        /// <response code="404">Không tìm thấy booking</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Hủy lượt đặt với lý do
        /// 
        /// **Ràng buộc:**
        /// - Chỉ có thể hủy booking ở trạng thái Draft, Pending_Approval hoặc Approved
        /// - Phải hủy trước 2 giờ từ thời gian bắt đầu (StartTime)
        /// - Chỉ có thể hủy booking của chính mình
        /// 
        /// **Sau khi hủy:**
        /// - Booking sẽ chuyển sang trạng thái Cancelled
        /// - Slot sẽ được giải phóng và người khác có thể đặt được
        /// - Admin sẽ nhận thông báo về việc hủy booking
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 403)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> Cancel(string id, [FromQuery] string? reason = null)
        {
            try
            {
                // Lấy userId từ JWT token
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                            User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse.Fail(401, "Không tìm thấy user id trong token."));
                }

                var result = await _bookingService.CancelAsync(id, userId, reason);
                if (!result.Success)
                {
                    if (result.Error?.Code == 400)
                        return BadRequest(result);
                    if (result.Error?.Code == 403)
                        return StatusCode(403, result);
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
        /// Kiểm tra availability của facility trước khi đặt
        /// </summary>
        /// <param name="dto">Thông tin facility và thời gian</param>
        /// <returns>Kết quả kiểm tra availability và danh sách facility thay thế</returns>
        /// <response code="200">Trả về kết quả kiểm tra</response>
        /// <response code="404">Không tìm thấy facility</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Kiểm tra real-time xem facility có available không trước khi đặt
        /// 
        /// **Response:**
        /// - isAvailable: true nếu facility sẵn sàng
        /// - conflictReason: Lý do nếu không available (trùng lịch, phòng hỏng)
        /// - alternativeFacilities: Danh sách facility thay thế nếu không available
        /// 
        /// **Use Cases:**
        /// - User đang đặt phòng nhưng phòng bị trùng lịch → Suggest alternatives
        /// - Phòng hỏng (Under_Maintenance) → Suggest alternatives hoặc next available time
        /// </remarks>
        [HttpPost("check-availability")]
        [ProducesResponseType(typeof(ApiResponse<AvailabilityCheckResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> CheckAvailability([FromBody] CheckAvailabilityDto dto)
        {
            try
            {
                var result = await _bookingService.CheckAvailabilityAsync(dto);
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
        /// Lấy danh sách facility thay thế khi facility không available
        /// </summary>
        /// <param name="facilityId">Facility ID không available</param>
        /// <param name="startTime">Thời gian bắt đầu</param>
        /// <param name="endTime">Thời gian kết thúc</param>
        /// <param name="capacity">Sức chứa tối thiểu</param>
        /// <returns>Danh sách facility thay thế</returns>
        /// <response code="200">Trả về danh sách alternatives</response>
        /// <response code="404">Không tìm thấy facility</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Tìm facility thay thế trong cùng campus, cùng type, capacity >= yêu cầu
        /// 
        /// **Logic:**
        /// - Tìm facilities cùng campus, cùng type
        /// - Capacity >= yêu cầu
        /// - Available trong khoảng thời gian
        /// - Nếu không available, suggest next available time
        /// </remarks>
        [HttpGet("alternatives")]
        [ProducesResponseType(typeof(ApiResponse<List<AlternativeFacilityDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetAlternatives(
            [FromQuery] string facilityId,
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime,
            [FromQuery] int capacity = 1)
        {
            try
            {
                var result = await _bookingService.GetAlternativeFacilitiesAsync(
                    facilityId,
                    startTime,
                    endTime,
                    capacity
                );
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
        /// Check-in booking (ghi nhận thời gian đến sử dụng)
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking đã check-in</returns>
        /// <response code="200">Check-in thành công</response>
        /// <response code="400">Booking không ở trạng thái Approved hoặc đã check-in</response>
        /// <response code="403">Không có quyền check-in booking này</response>
        /// <response code="404">Không tìm thấy booking</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Ghi nhận thời gian user đến sử dụng facility
        /// 
        /// **Điều kiện:**
        /// - Booking phải ở trạng thái Approved
        /// - Booking chưa được check-in
        /// - Chỉ có thể check-in từ 15 phút trước StartTime đến EndTime
        /// - Chỉ chủ booking mới có thể check-in
        /// 
        /// **Lưu ý:** 
        /// - CheckInTime sẽ được set bằng thời gian hiện tại (Vietnam time)
        /// - Sau khi check-in, có thể check-out để hoàn tất booking
        /// </remarks>
        [HttpPost("{id}/check-in")]
        [ProducesResponseType(typeof(ApiResponse<BookingResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 403)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> CheckIn(string id)
        {
            try
            {
                // Lấy userId từ JWT token
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                            User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse.Fail(401, "Không tìm thấy user id trong token."));
                }

                var result = await _bookingService.CheckInAsync(id, userId);
                if (!result.Success)
                {
                    if (result.Error?.Code == 400)
                        return BadRequest(result);
                    if (result.Error?.Code == 403)
                        return StatusCode(403, result);
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
        /// Check-out booking (ghi nhận thời gian rời đi)
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking đã check-out</returns>
        /// <response code="200">Check-out thành công</response>
        /// <response code="400">Chưa check-in hoặc đã check-out</response>
        /// <response code="403">Không có quyền check-out booking này</response>
        /// <response code="404">Không tìm thấy booking</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Ghi nhận thời gian user rời khỏi facility
        /// 
        /// **Điều kiện:**
        /// - Booking phải đã được check-in
        /// - Booking chưa được check-out
        /// - Thời gian check-out phải sau thời gian check-in
        /// - Chỉ chủ booking mới có thể check-out
        /// 
        /// **Lưu ý:** 
        /// - CheckOutTime sẽ được set bằng thời gian hiện tại (Vietnam time)
        /// - Nếu check-out sau EndTime, status sẽ tự động chuyển thành Completed
        /// </remarks>
        [HttpPost("{id}/check-out")]
        [ProducesResponseType(typeof(ApiResponse<BookingResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 403)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> CheckOut(string id)
        {
            try
            {
                // Lấy userId từ JWT token
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                            User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse.Fail(401, "Không tìm thấy user id trong token."));
                }

                var result = await _bookingService.CheckOutAsync(id, userId);
                if (!result.Success)
                {
                    if (result.Error?.Code == 400)
                        return BadRequest(result);
                    if (result.Error?.Code == 403)
                        return StatusCode(403, result);
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
