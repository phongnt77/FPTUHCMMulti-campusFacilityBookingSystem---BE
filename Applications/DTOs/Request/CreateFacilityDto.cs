using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateFacilityDto
    {
        [Required(ErrorMessage = "Tên cơ sở là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Sức chứa là bắt buộc")]
        public int Capacity { get; set; }

        public string? RoomNumber { get; set; }
        public string? FloorNumber { get; set; }

        [Required(ErrorMessage = "Cơ sở là bắt buộc")]
        public string CampusId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại cơ sở vật chất là bắt buộc")]
        public string TypeId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public string Status { get; set; } = string.Empty;

        public string? Amenities { get; set; }
        public string? FacilityManagerId { get; set; }
        public int MaxConcurrentBookings { get; set; } = 1;
    }
}


