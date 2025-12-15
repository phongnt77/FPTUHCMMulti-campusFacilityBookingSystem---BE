using Applications.DTOs.Request;
using Applications.DTOs.Response;
using Applications.Helpers;
using BLL.Interfaces;
using DAL.Repositories;

namespace BLL.Classes
{
    public class SystemSettingsService : ISystemSettingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const string KEY_MIN_BOOKING_HOURS = "MinimumBookingHoursBeforeStart";
        private const string KEY_CHECKIN_MINUTES_BEFORE = "CheckInMinutesBeforeStart";
        private const string KEY_CHECKIN_MINUTES_AFTER = "CheckInMinutesAfterStart";

        // thời gian mặc định
        private const int DEFAULT_MIN_BOOKING_HOURS = 3;
        private const int DEFAULT_CHECKIN_MINUTES_BEFORE = 15;
        private const int DEFAULT_CHECKIN_MINUTES_AFTER = 15;

        public SystemSettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<SystemSettingsResponseDto>> GetSettingsAsync()
        {
            var minHours = await GetMinimumBookingHoursBeforeStartAsync();
            var checkInBefore = await GetCheckInMinutesBeforeStartAsync();
            var checkInAfter = await GetCheckInMinutesAfterStartAsync();

            var response = new SystemSettingsResponseDto
            {
                MinimumBookingHoursBeforeStart = minHours,
                CheckInMinutesBeforeStart = checkInBefore,
                CheckInMinutesAfterStart = checkInAfter
            };

            return ApiResponse<SystemSettingsResponseDto>.Ok(response);
        }

        public async Task<ApiResponse<SystemSettingsResponseDto>> UpdateSettingsAsync(UpdateSystemSettingsDto dto)
        {
            if (dto.MinimumBookingHoursBeforeStart.HasValue)
            {
                if (dto.MinimumBookingHoursBeforeStart.Value < 0)
                {
                    return ApiResponse<SystemSettingsResponseDto>.Fail(400, "Thời gian tối thiểu trước khi booking bắt đầu phải >= 0.");
                }
                await _unitOfWork.SystemSettingsRepo.CreateOrUpdateAsync(
                    KEY_MIN_BOOKING_HOURS,
                    dto.MinimumBookingHoursBeforeStart.Value.ToString(),
                    "Thời gian tối thiểu (giờ) trước khi booking bắt đầu"
                );
            }

            if (dto.CheckInMinutesBeforeStart.HasValue)
            {
                if (dto.CheckInMinutesBeforeStart.Value < 0)
                {
                    return ApiResponse<SystemSettingsResponseDto>.Fail(400, "Số phút check-in trước StartTime phải >= 0.");
                }
                await _unitOfWork.SystemSettingsRepo.CreateOrUpdateAsync(
                    KEY_CHECKIN_MINUTES_BEFORE,
                    dto.CheckInMinutesBeforeStart.Value.ToString(),
                    "Số phút cho phép check-in trước StartTime"
                );
            }

            if (dto.CheckInMinutesAfterStart.HasValue)
            {
                if (dto.CheckInMinutesAfterStart.Value < 0)
                {
                    return ApiResponse<SystemSettingsResponseDto>.Fail(400, "Số phút check-in sau StartTime phải >= 0.");
                }
                await _unitOfWork.SystemSettingsRepo.CreateOrUpdateAsync(
                    KEY_CHECKIN_MINUTES_AFTER,
                    dto.CheckInMinutesAfterStart.Value.ToString(),
                    "Số phút cho phép check-in sau StartTime"
                );
            }

            await _unitOfWork.SaveChangesAsync();

            return await GetSettingsAsync();
        }

        public async Task<int> GetMinimumBookingHoursBeforeStartAsync()
        {
            var setting = await _unitOfWork.SystemSettingsRepo.GetByKeyAsync(KEY_MIN_BOOKING_HOURS);
            if (setting != null && int.TryParse(setting.SettingValue, out var value))
            {
                return value;
            }
            return DEFAULT_MIN_BOOKING_HOURS;
        }

        public async Task<int> GetCheckInMinutesBeforeStartAsync()
        {
            var setting = await _unitOfWork.SystemSettingsRepo.GetByKeyAsync(KEY_CHECKIN_MINUTES_BEFORE);
            if (setting != null && int.TryParse(setting.SettingValue, out var value))
            {
                return value;
            }
            return DEFAULT_CHECKIN_MINUTES_BEFORE;
        }

        public async Task<int> GetCheckInMinutesAfterStartAsync()
        {
            var setting = await _unitOfWork.SystemSettingsRepo.GetByKeyAsync(KEY_CHECKIN_MINUTES_AFTER);
            if (setting != null && int.TryParse(setting.SettingValue, out var value))
            {
                return value;
            }
            return DEFAULT_CHECKIN_MINUTES_AFTER;
        }
    }
}

