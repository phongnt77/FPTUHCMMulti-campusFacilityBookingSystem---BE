namespace Applications.DTOs.Request
{
    public class UpdateSystemSettingsDto
    {
        /// <summary>
        /// thời gian tối thiểu (giờ) trước khi booking bắt đầu (mặc định: 3)
        /// </summary>
        public int? MinimumBookingHoursBeforeStart { get; set; }

        /// <summary>
        /// số phút cho phép check-in trước StartTime (mặc định: 15)
        /// </summary>
        public int? CheckInMinutesBeforeStart { get; set; }

        /// <summary>
        /// số phút cho phép check-in sau StartTime (mặc định: 15)
        /// </summary>
        public int? CheckInMinutesAfterStart { get; set; }

        /// <summary>
        /// Tỉ lệ tối thiểu (0-1) thời lượng booking phải qua mới được phép check-out (mặc định: 2/3).
        /// Ví dụ: 0.66 nghĩa là phải qua 66% thời gian booking mới được check-out.
        /// </summary>
        public double? CheckoutMinRatio { get; set; }
    }
}

