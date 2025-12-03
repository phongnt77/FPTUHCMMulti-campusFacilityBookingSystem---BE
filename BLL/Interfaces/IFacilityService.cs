using Applications.DTOs.Request;
using Applications.DTOs.Response;

namespace BLL.Interfaces
{
    public interface IFacilityService
    {
        Task<ApiResponseWithPagination<List<FacilityResponseDto>>> GetAllAsync(FacilityFilterDto filter);
        Task<ApiResponse<FacilityResponseDto>> GetByIdAsync(string id);
        Task<ApiResponse<FacilityResponseDto>> CreateAsync(CreateFacilityDto dto);
        Task<ApiResponse<FacilityResponseDto>> UpdateAsync(string id, UpdateFacilityDto dto);
        Task<ApiResponse> DeleteAsync(string id);
    }
}


