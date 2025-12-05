using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateBookingFeedbackDto
    {
        [Required(ErrorMessage = "ID lượt đặt là bắt buộc")]
        public string BookingId { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID người dùng là bắt buộc")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Đánh giá là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        public int Rating { get; set; }

        public string? Comments { get; set; }
        public bool ReportIssue { get; set; } = false;
        public string? IssueDescription { get; set; }
    }
}

