using DAL.Models.Enums;

namespace DAL.Models
{
    public class Report
    {
        public string ReportId { get; set; } = string.Empty;
        public string ReportName { get; set; } = string.Empty;
        public ReportType ReportType { get; set; }
        public string CampusId { get; set; } = string.Empty;
        public string? FacilityId { get; set; }
        public DateTime ReportPeriodStart { get; set; }
        public DateTime ReportPeriodEnd { get; set; }
        public PeriodType PeriodType { get; set; }
        public int TotalBookings { get; set; } = 0;
        public int ApprovedBookings { get; set; } = 0;
        public int RejectedBookings { get; set; } = 0;
        public int CompletedBookings { get; set; } = 0;
        public decimal UtilizationRate { get; set; } = 0;
        public decimal AvgRating { get; set; } = 0;
        public int UniqueUsers { get; set; } = 0;
        public string? PeakHours { get; set; } // JSON string
        public string? BusiestDay { get; set; }
        public string GeneratedBy { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublished { get; set; } = false;
        public string? Notes { get; set; }

        // Navigation properties
        public Campus Campus { get; set; } = null!;
        public Facility? Facility { get; set; }
        public User GeneratedByUser { get; set; } = null!;
    }
}

