namespace Applications.DTOs.Response
{
    public class BookingResponseDto
    {
        public string BookingId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FacilityId { get; set; } = string.Empty;
        public string FacilityName { get; set; } = string.Empty;
        public string? FacilityRoomNumber { get; set; }
        public string? FacilityFloorNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Purpose { get; set; }
        public string? Category { get; set; }
        public int? EstimatedAttendees { get; set; }
        public string? SpecialRequirements { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

