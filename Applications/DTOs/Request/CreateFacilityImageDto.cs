using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateFacilityImageDto
    {
        [Required(ErrorMessage = "ID cơ sở vật chất là bắt buộc")]
        public string FacilityId { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL hình ảnh là bắt buộc")]
        public string ImageUrl { get; set; } = string.Empty;

        public string? Description { get; set; }
        public byte? ImageOrder { get; set; }
    }
}

