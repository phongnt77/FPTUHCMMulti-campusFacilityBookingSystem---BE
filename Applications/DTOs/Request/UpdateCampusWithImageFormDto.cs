using DAL.Models.Enums;

namespace Applications.DTOs.Request
{
    /// <summary>
    /// Form-data DTO for updating campus with an uploaded image.
    /// Intentionally does NOT include ImageUrl; server sets it after Cloudinary upload.
    /// </summary>
    public class UpdateCampusWithImageFormDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public CampusStatus? Status { get; set; }
    }
}
