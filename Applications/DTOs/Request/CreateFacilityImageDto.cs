using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateFacilityImageDto
    {
        [Required]
        public string FacilityId { get; set; } = string.Empty;

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public string? Description { get; set; }
        public byte? ImageOrder { get; set; }
    }
}

