using DAL.Models.Enums;

namespace DAL.Models
{
    public class FacilityMaintenance
    {
        public string MaintenanceId { get; set; } = string.Empty;
        public string FacilityId { get; set; } = string.Empty;
        public string? IssueType { get; set; }
        public string? Description { get; set; }
        public MaintenancePriority Priority { get; set; } = MaintenancePriority.Medium;
        public MaintenanceStatus Status { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime? ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }
        public string? CompletionNotes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Facility Facility { get; set; } = null!;
        public User? AssignedToUser { get; set; }
    }
}

