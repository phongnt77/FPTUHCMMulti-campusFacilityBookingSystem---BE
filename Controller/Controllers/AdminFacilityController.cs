using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Controllers
{
    /// <summary>
    /// API quản lý facilities (Admin only)
    /// </summary>
    [ApiController]
    [Route("api/admin/facilities")]
    [Authorize]
    public class AdminFacilityController : ControllerBase
    {
        private readonly IReportService _reportService;

        public AdminFacilityController(IReportService reportService)
        {
            _reportService = reportService;
        }

        
        [HttpGet("usage-report")]
        [ProducesResponseType(typeof(ApiResponse<UsageReportResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        public async Task<IActionResult> GetUsageReport(
            [FromQuery] string? campusId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            try
            {
                var request = new UsageReportRequestDto
                {
                    CampusId = campusId,
                    From = from,
                    To = to
                };

                var result = await _reportService.GetFacilityUsageReportAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }
    }
}

