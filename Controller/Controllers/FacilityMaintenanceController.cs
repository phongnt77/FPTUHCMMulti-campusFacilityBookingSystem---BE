using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Controllers
{
    /// <summary>
    /// API quản lý bảo trì cơ sở vật chất (Facility Maintenance)
    /// </summary>
    [ApiController]
    [Route("api/facility-maintenances")]
    [Authorize]
    public class FacilityMaintenanceController : ControllerBase
    {
        private readonly IFacilityMaintenanceService _maintenanceService;

        public FacilityMaintenanceController(IFacilityMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        /// <summary>
        /// Lấy danh sách bảo trì với filtering
        /// </summary>
        /// <param name="facilityId">Lọc theo facility (optional)</param>
        /// <param name="status">Lọc theo trạng thái: Scheduled | In_Progress | Completed (optional)</param>
        /// <param name="request">Pagination parameters</param>
        /// <returns>Danh sách bảo trì</returns>
        /// <response code="200">Trả về danh sách thành công</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Xem lịch bảo trì facilities
        /// 
        /// **Status values:**
        /// - Scheduled: Đã lên lịch
        /// - In_Progress: Đang bảo trì
        /// - Completed: Hoàn thành
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithPagination<List<MaintenanceResponseDto>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] string? facilityId, [FromQuery] string? status, [FromQuery] PagedRequestDto request)
        {
            try
            {
                var result = await _maintenanceService.GetAllAsync(facilityId, status, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        /// <summary>
        /// Lấy chi tiết bản ghi bảo trì
        /// </summary>
        /// <param name="id">Maintenance ID</param>
        /// <returns>Thông tin bảo trì</returns>
        /// <response code="200">Trả về thông tin thành công</response>
        /// <response code="404">Không tìm thấy bản ghi</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// </remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<MaintenanceResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var result = await _maintenanceService.GetByIdAsync(id);
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
        /// Tạo lịch bảo trì mới
        /// </summary>
        /// <param name="dto">Thông tin bảo trì</param>
        /// <returns>Bản ghi bảo trì đã tạo</returns>
        /// <response code="200">Tạo thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Lên lịch bảo trì facility
        /// 
        /// **Priority:** Low | Medium | High
        /// 
        /// **Status:** Scheduled | In_Progress | Completed
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<MaintenanceResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateMaintenanceDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail(400, "Dữ liệu không hợp lệ."));
            }

            try
            {
                var result = await _maintenanceService.CreateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }
    }
}
