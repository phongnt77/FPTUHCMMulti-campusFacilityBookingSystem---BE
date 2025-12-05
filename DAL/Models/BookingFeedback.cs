namespace DAL.Models
{
    public class BookingFeedback
    {
        public string FeedbackId { get; set; } = string.Empty;
        public string BookingId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comments { get; set; }
        public bool ReportIssue { get; set; } = false;
        public string? IssueDescription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }

        // Navigation properties
        public Booking Booking { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}

