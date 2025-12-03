using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateMaintenanceDto
    {
        [Required]
        public string FacilityId { get; set; } = string.Empty;

        public string? IssueType { get; set; }
        public string? Description { get; set; }

        [Required]
        public string Priority { get; set; } = "Medium";

        [Required]
        public string Status { get; set; } = "Scheduled";

        public string? AssignedTo { get; set; }
        public DateTime? ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }
    }
}

