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
    [Route("api/admin/bookings")]
    [Authorize]
    public class AdminBookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public AdminBookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            try
            {
                // chuyển đổi "PENDING" thành "Pending_Approval" 
                var mappedStatus = status?.ToUpper() == "PENDING" ? "Pending_Approval" : status;
                
                var filter = new BookingFilterDto
                {
                    Status = mappedStatus,
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

        [HttpPatch("{bookingId}/approve")]
        public async Task<IActionResult> ApproveBooking(string bookingId)
        {
            var approverId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                            User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(approverId))
            {
                return Unauthorized(ApiResponse.Fail(401, "User id is missing in token."));
            }

            try
            {
                var result = await _bookingService.ApproveBookingAsync(bookingId, approverId);
                
                if (!result.Success)
                {
                    if (result.Error?.Code == 404)
                        return NotFound(result);
                    if (result.Error?.Code == 400)
                        return BadRequest(result);
                    if (result.Error?.Code == 409)
                        return Conflict(result);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        [HttpPatch("{bookingId}/reject")]
        public async Task<IActionResult> RejectBooking(string bookingId, [FromBody] RejectBookingDto? dto = null)
        {
            var approverId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                            User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(approverId))
            {
                return Unauthorized(ApiResponse.Fail(401, "User id is missing in token."));
            }

            try
            {
                var reason = dto?.Reason;
                var result = await _bookingService.RejectBookingAsync(bookingId, approverId, reason);
                
                if (!result.Success)
                {
                    if (result.Error?.Code == 404)
                        return NotFound(result);
                    if (result.Error?.Code == 400)
                        return BadRequest(result);
                    return BadRequest(result);
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

