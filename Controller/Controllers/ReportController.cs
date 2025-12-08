using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Controllers
{
    /// <summary>
    /// API báo cáo thống kê bookings
    /// </summary>
    [ApiController]
    [Route("api/reports")]
    [Authorize(Roles = "RL0003")] // Chỉ Facility_Admin
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Lấy báo cáo thống kê bookings với filter thời gian
        /// </summary>
        /// <param name="filter">Bộ lọc báo cáo</param>
        /// <returns>Báo cáo thống kê chi tiết</returns>
        /// <response code="200">Trả về báo cáo</response>
        /// <remarks>
        /// **Roles:** Chỉ Facility_Admin (RL0003)
        /// 
        /// **Mục đích:** Thống kê lượt đặt, tỷ lệ sử dụng với filter thời gian
        /// 
        /// **Period Types:**
        /// - "day": X ngày gần nhất (dùng với days: 1, 3, 7, 14, 30)
        /// - "week": 7 ngày gần nhất
        /// - "month": Theo tháng (dùng với month: 1-12, year: 2024, 2025...)
        /// - "year": Theo năm (dùng với year: 2024, 2025...)
        /// - "custom": Khoảng thời gian tùy chỉnh (dùng với startDate, endDate)
        /// 
        /// **Filters:**
        /// - periodType: Loại period (day/week/month/year/custom)
        /// - days: Số ngày (1, 3, 7, 14, 30) - dùng với periodType=day
        /// - month: Tháng (1-12) - dùng với periodType=month
        /// - year: Năm (2024, 2025...) - dùng với periodType=month hoặc year
        /// - startDate: Ngày bắt đầu - dùng với periodType=custom
        /// - endDate: Ngày kết thúc - dùng với periodType=custom
        /// - campusId: Filter theo campus (optional)
        /// - facilityId: Filter theo facility (optional)
        /// 
        /// **Response Includes:**
        /// - Period info: Thông tin khoảng thời gian
        /// - Overall statistics: Tổng quan (total bookings, approval rate, utilization rate...)
        /// - Daily statistics: Thống kê theo ngày
        /// - Facility statistics: Thống kê theo facility (với rating)
        /// - Campus statistics: Thống kê theo campus
        /// 
        /// **Examples:**
        /// - Last 7 days: periodType=day&days=7
        /// - Last 30 days: periodType=day&days=30
        /// - December 2024: periodType=month&month=12&year=2024
        /// - Year 2024: periodType=year&year=2024
        /// - Custom: periodType=custom&startDate=2024-01-01&endDate=2024-12-31
        /// </remarks>
        [HttpGet("bookings")]
        [ProducesResponseType(typeof(ApiResponse<BookingReportResponseDto>), 200)]
        public async Task<IActionResult> GetBookingReport([FromQuery] BookingReportFilterDto filter)
        {
            try
            {
                var result = await _reportService.GetBookingReportAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }
    }
}

