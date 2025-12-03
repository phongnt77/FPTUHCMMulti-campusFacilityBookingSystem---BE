using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateBookingFeedbackDto
    {
        [Required]
        public string BookingId { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comments { get; set; }
        public bool ReportIssue { get; set; } = false;
        public string? IssueDescription { get; set; }
    }
}

