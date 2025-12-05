namespace DAL.Models
{
    public class FacilityImage
    {
        public string ImageId { get; set; } = string.Empty;
        public string FacilityId { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public byte? ImageOrder { get; set; }

        // Navigation properties
        public Facility Facility { get; set; } = null!;
    }
}

