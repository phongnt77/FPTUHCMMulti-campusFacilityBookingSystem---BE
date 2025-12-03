using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateFacilityDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int Capacity { get; set; }

        public string? RoomNumber { get; set; }
        public string? FloorNumber { get; set; }

        [Required]
        public string CampusId { get; set; } = string.Empty;

        [Required]
        public string TypeId { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? Amenities { get; set; }
        public string? FacilityManagerId { get; set; }
        public int MaxConcurrentBookings { get; set; } = 1;
    }
}


