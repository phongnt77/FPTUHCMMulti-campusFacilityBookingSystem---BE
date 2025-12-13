using Applications.DTOs.Request;
using Applications.DTOs.Response;

namespace BLL.Interfaces
{
    public interface ISystemSettingsService
    {
        Task<ApiResponse<SystemSettingsResponseDto>> GetSettingsAsync();
        Task<ApiResponse<SystemSettingsResponseDto>> UpdateSettingsAsync(UpdateSystemSettingsDto dto);
        
        // Helper methods để lấy giá trị settings (dùng trong BookingService)
        Task<int> GetMinimumBookingHoursBeforeStartAsync();
        Task<int> GetCheckInMinutesBeforeStartAsync();
        Task<int> GetCheckInMinutesAfterStartAsync();
    }
}

