using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateCampusDto
    {
        [Required(ErrorMessage = "Tên cơ sở là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? FacilityManagerId { get; set; }
    }
}


