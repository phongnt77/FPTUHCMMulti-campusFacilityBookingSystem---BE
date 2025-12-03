using Applications.DTOs.Request;
using Applications.DTOs.Response;

namespace BLL.Interfaces
{
    public interface IFacilityImageService
    {
        Task<ApiResponse<List<FacilityImageResponseDto>>> GetByFacilityIdAsync(string facilityId);
        Task<ApiResponse<FacilityImageResponseDto>> CreateAsync(CreateFacilityImageDto dto);
        Task<ApiResponse> DeleteAsync(string id);
    }
}

