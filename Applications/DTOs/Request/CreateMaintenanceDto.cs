using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateMaintenanceDto
    {
        [Required(ErrorMessage = "ID cơ sở vật chất là bắt buộc")]
        public string FacilityId { get; set; } = string.Empty;

        public string? IssueType { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Mức độ ưu tiên là bắt buộc")]
        public string Priority { get; set; } = "Medium";

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public string Status { get; set; } = "Scheduled";

        public string? AssignedTo { get; set; }
        public DateTime? ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }
    }
}

