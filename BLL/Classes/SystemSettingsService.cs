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
        private const string KEY_CHECKOUT_MIN_RATIO = "CheckoutMinRatio";

        // thời gian mặc định
        private const int DEFAULT_MIN_BOOKING_HOURS = 3;
        private const int DEFAULT_CHECKIN_MINUTES_BEFORE = 15;
        private const int DEFAULT_CHECKIN_MINUTES_AFTER = 15;
        // mặc định cho phép check-out sau khi đã qua 2/3 thời lượng booking
        private const double DEFAULT_CHECKOUT_MIN_RATIO = 2.0 / 3.0;

        public SystemSettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// lấy tất cả cấu hình hệ thống hiện tại
        /// hàm này trả về tất cả các setting có thể config được, bao gồm cả giá trị mặc định nếu chưa được set
        /// </summary>
        public async Task<ApiResponse<SystemSettingsResponseDto>> GetSettingsAsync()
        {
            // lấy từng setting từ database hoặc giá trị mặc định
            // mỗi hàm get sẽ tự động trả về giá trị mặc định nếu setting chưa được config
            var minHours = await GetMinimumBookingHoursBeforeStartAsync();
            var checkInBefore = await GetCheckInMinutesBeforeStartAsync();
            var checkInAfter = await GetCheckInMinutesAfterStartAsync();
            var checkoutRatio = await GetCheckoutMinRatioAsync();

            // tạo response dto với tất cả settings
            var response = new SystemSettingsResponseDto
            {
                MinimumBookingHoursBeforeStart = minHours,
                CheckInMinutesBeforeStart = checkInBefore,
                CheckInMinutesAfterStart = checkInAfter,
                CheckoutMinRatio = checkoutRatio
            };

            return ApiResponse<SystemSettingsResponseDto>.Ok(response);
        }

        /// <summary>
        /// cập nhật cấu hình hệ thống (chỉ admin mới có quyền)
        /// hàm này cho phép admin thay đổi các ràng buộc thời gian của hệ thống
        /// chỉ cần gửi các field muốn cập nhật, các field khác sẽ giữ nguyên
        /// </summary>
        public async Task<ApiResponse<SystemSettingsResponseDto>> UpdateSettingsAsync(UpdateSystemSettingsDto dto)
        {
            // cập nhật minimumbookinghoursbeforestart (thời gian tối thiểu trước khi booking bắt đầu)
            // ví dụ: nếu set = 3 giờ, user phải đặt booking trước 3 giờ từ starttime
            if (dto.MinimumBookingHoursBeforeStart.HasValue)
            {
                // validate giá trị phải >= 0
                if (dto.MinimumBookingHoursBeforeStart.Value < 0)
                {
                    return ApiResponse<SystemSettingsResponseDto>.Fail(400, "Thời gian tối thiểu trước khi booking bắt đầu phải >= 0.");
                }
                // createorupdateasync sẽ tự động tạo mới nếu chưa có, hoặc update nếu đã có
                await _unitOfWork.SystemSettingsRepo.CreateOrUpdateAsync(
                    KEY_MIN_BOOKING_HOURS,
                    dto.MinimumBookingHoursBeforeStart.Value.ToString(),
                    "Thời gian tối thiểu (giờ) trước khi booking bắt đầu"
                );
            }

            // cập nhật checkinminutesbeforestart (số phút cho phép check-in trước starttime)
            // ví dụ: nếu set = 15 phút, booking 9h thì có thể check-in từ 8h45
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

            // cập nhật checkinminutesafterstart (số phút cho phép check-in sau starttime)
            // ví dụ: nếu set = 15 phút, booking 9h thì có thể check-in đến 9h15
            // nếu quá thời gian này, booking sẽ bị hủy tự động bởi background service
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

            // cập nhật checkoutminratio (tỉ lệ tối thiểu thời lượng booking phải qua để cho phép check-out)
            // ví dụ: nếu set = 0.6667 (2/3), booking 3 giờ thì phải đợi 2 giờ mới được checkout
            // giá trị phải trong khoảng [0, 1] (0 = 0%, 1 = 100%)
            if (dto.CheckoutMinRatio.HasValue)
            {
                if (dto.CheckoutMinRatio.Value < 0 || dto.CheckoutMinRatio.Value > 1)
                {
                    return ApiResponse<SystemSettingsResponseDto>.Fail(400, "Tỉ lệ thời gian check-out phải nằm trong khoảng 0 - 1.");
                }

                // sử dụng invariantculture để đảm bảo lưu với dấu chấm (0.6667) thay vì dấu phẩy
                // tránh lỗi khi parse lại sau này
                await _unitOfWork.SystemSettingsRepo.CreateOrUpdateAsync(
                    KEY_CHECKOUT_MIN_RATIO,
                    dto.CheckoutMinRatio.Value.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    "Tỉ lệ tối thiểu (0-1) thời lượng booking phải qua để cho phép check-out (mặc định 2/3)"
                );
            }

            // lưu tất cả thay đổi vào database
            await _unitOfWork.SaveChangesAsync();

            // trả về settings hiện tại sau khi cập nhật
            return await GetSettingsAsync();
        }

        /// <summary>
        /// lấy thời gian tối thiểu (giờ) trước khi booking bắt đầu
        /// nếu chưa có setting trong database, trả về giá trị mặc định (3 giờ)
        /// </summary>
        public async Task<int> GetMinimumBookingHoursBeforeStartAsync()
        {
            // query setting từ database
            var setting = await _unitOfWork.SystemSettingsRepo.GetByKeyAsync(KEY_MIN_BOOKING_HOURS);
            // nếu có setting và parse thành công, trả về giá trị từ database
            if (setting != null && int.TryParse(setting.SettingValue, out var value))
            {
                return value;
            }
            // nếu không có hoặc parse lỗi, trả về giá trị mặc định
            return DEFAULT_MIN_BOOKING_HOURS;
        }

        /// <summary>
        /// lấy số phút cho phép check-in trước starttime
        /// nếu chưa có setting trong database, trả về giá trị mặc định (15 phút)
        /// </summary>
        public async Task<int> GetCheckInMinutesBeforeStartAsync()
        {
            var setting = await _unitOfWork.SystemSettingsRepo.GetByKeyAsync(KEY_CHECKIN_MINUTES_BEFORE);
            if (setting != null && int.TryParse(setting.SettingValue, out var value))
            {
                return value;
            }
            return DEFAULT_CHECKIN_MINUTES_BEFORE;
        }

        /// <summary>
        /// lấy số phút cho phép check-in sau starttime
        /// nếu chưa có setting trong database, trả về giá trị mặc định (15 phút)
        /// giá trị này được dùng bởi background service để tự động hủy booking quá thời gian
        /// </summary>
        public async Task<int> GetCheckInMinutesAfterStartAsync()
        {
            var setting = await _unitOfWork.SystemSettingsRepo.GetByKeyAsync(KEY_CHECKIN_MINUTES_AFTER);
            if (setting != null && int.TryParse(setting.SettingValue, out var value))
            {
                return value;
            }
            return DEFAULT_CHECKIN_MINUTES_AFTER;
        }

        /// <summary>
        /// lấy tỉ lệ tối thiểu (0-1) thời lượng booking phải qua để cho phép check-out
        /// nếu chưa có setting trong database, trả về giá trị mặc định (2/3 = 0.6667)
        /// sử dụng invariantculture để parse đúng với format số thập phân (dấu chấm)
        /// </summary>
        public async Task<double> GetCheckoutMinRatioAsync()
        {
            var setting = await _unitOfWork.SystemSettingsRepo.GetByKeyAsync(KEY_CHECKOUT_MIN_RATIO);
            // sử dụng invariantculture để đảm bảo parse đúng với format đã lưu (dấu chấm)
            if (setting != null && double.TryParse(setting.SettingValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }
            return DEFAULT_CHECKOUT_MIN_RATIO;
        }
    }
}

