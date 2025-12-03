using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateCampusDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? FacilityManagerId { get; set; }
    }
}


