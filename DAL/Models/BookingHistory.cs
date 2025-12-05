namespace DAL.Models
{
    public class BookingHistory
    {
        public string HistoryId { get; set; } = string.Empty;
        public string BookingId { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
        public string? FieldChanged { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Reason { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Booking Booking { get; set; } = null!;
        public User ModifiedByUser { get; set; } = null!;
    }
}

