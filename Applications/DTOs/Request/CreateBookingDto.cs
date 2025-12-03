using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateBookingDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string FacilityId { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string? Purpose { get; set; }
        public string? Category { get; set; }
        public int? EstimatedAttendees { get; set; }
        public string? SpecialRequirements { get; set; }
    }
}

