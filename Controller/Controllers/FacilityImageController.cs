using Applications.DTOs.Request;
using Applications.DTOs.Response;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Controllers
{
    /// <summary>
    /// API quản lý hình ảnh cơ sở vật chất (Facility Images)
    /// </summary>
    [ApiController]
    [Route("api/facility-images")]
    [Authorize]
    public class FacilityImageController : ControllerBase
    {
        private readonly IFacilityImageService _imageService;

        public FacilityImageController(IFacilityImageService imageService)
        {
            _imageService = imageService;
        }

        /// <summary>
        /// Lấy tất cả hình ảnh của facility
        /// </summary>
        /// <param name="facilityId">Facility ID</param>
        /// <returns>Danh sách hình ảnh</returns>
        /// <response code="200">Trả về danh sách thành công</response>
        /// <remarks>
        /// **Roles:** Công khai - Không cần đăng nhập
        /// 
        /// **Mục đích:** Hiển thị gallery hình ảnh của facility
        /// 
        /// Hình ảnh được sắp xếp theo image_order
        /// </remarks>
        [HttpGet("facility/{facilityId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<List<FacilityImageResponseDto>>), 200)]
        public async Task<IActionResult> GetByFacilityId(string facilityId)
        {
            try
            {
                var result = await _imageService.GetByFacilityIdAsync(facilityId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        /// <summary>
        /// Upload hình ảnh mới cho facility
        /// </summary>
        /// <param name="dto">Thông tin hình ảnh</param>
        /// <returns>Hình ảnh đã upload</returns>
        /// <response code="200">Upload thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Thêm hình ảnh cho facility
        /// 
        /// **imageOrder:** Thứ tự hiển thị (1, 2, 3, ...)
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<FacilityImageResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateFacilityImageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail(400, "Dữ liệu không hợp lệ."));
            }

            try
            {
                var result = await _imageService.CreateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail(500, ex.Message));
            }
        }

        /// <summary>
        /// Xóa hình ảnh
        /// </summary>
        /// <param name="id">Image ID</param>
        /// <returns>Kết quả xóa</returns>
        /// <response code="200">Xóa thành công</response>
        /// <response code="404">Không tìm thấy hình ảnh</response>
        /// <remarks>
        /// **Roles:** Tất cả user đã đăng nhập
        /// 
        /// **Mục đích:** Xóa hình ảnh khỏi facility (hard delete)
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _imageService.DeleteAsync(id);
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
