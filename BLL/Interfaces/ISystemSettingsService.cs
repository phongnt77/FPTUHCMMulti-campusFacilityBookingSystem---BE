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

        /// <summary>
        /// Lấy tỉ lệ thời gian tối thiểu phải qua để được phép check-out (0-1).
        /// Mặc định: 2/3 (0.6666...).
        /// </summary>
        Task<double> GetCheckoutMinRatioAsync();
    }
}

