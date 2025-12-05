using Applications.DTOs.Request;
using Applications.DTOs.Response;

namespace BLL.Interfaces
{
    public interface IFacilityMaintenanceService
    {
        Task<ApiResponseWithPagination<List<MaintenanceResponseDto>>> GetAllAsync(string? facilityId, string? status, PagedRequestDto request);
        Task<ApiResponse<MaintenanceResponseDto>> GetByIdAsync(string id);
        Task<ApiResponse<MaintenanceResponseDto>> CreateAsync(CreateMaintenanceDto dto);
    }
}

