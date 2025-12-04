using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Controllers
{
    /// <summary>
    /// API quản lý đánh giá booking (Booking Feedbacks)
    /// </summary>
    [ApiController]
    [Route("api/booking-feedbacks")]
    [Authorize]
    public class BookingFeedbackController : ControllerBase
    {
        private readonly IBookingFeedbackService _feedbackService;

        public BookingFeedbackController(IBookingFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        /// <summary>
        /// Lấy danh sách tất cả đánh giá
        /// </summary>
        /// <param name="request">Pagination parameters</param>
        /// <returns>Danh sách đánh giá</returns>
        /// <response code="200">Trả về danh sách thành công</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Xem tất cả đánh giá về facilities
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithPagination<List<BookingFeedbackResponseDto>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] PagedRequestDto request)
        {
            try
            {
                var result = await _feedbackService.GetAllAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        /// <summary>
        /// Lấy chi tiết đánh giá
        /// </summary>
        /// <param name="id">Feedback ID</param>
        /// <returns>Thông tin đánh giá</returns>
        /// <response code="200">Trả về thông tin thành công</response>
        /// <response code="404">Không tìm thấy đánh giá</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// </remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BookingFeedbackResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var result = await _feedbackService.GetByIdAsync(id);
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
        /// Tạo đánh giá mới cho booking
        /// </summary>
        /// <param name="dto">Thông tin đánh giá</param>
        /// <returns>Đánh giá đã tạo</returns>
        /// <response code="200">Tạo thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** User đánh giá facility sau khi sử dụng
        /// 
        /// **Rating:** 1-5 sao
        /// 
        /// **Report Issue:** Có thể báo cáo vấn đề về facility
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<BookingFeedbackResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateBookingFeedbackDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail(400, "Dữ liệu không hợp lệ."));
            }

            try
            {
                var result = await _feedbackService.CreateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }
    }
}
