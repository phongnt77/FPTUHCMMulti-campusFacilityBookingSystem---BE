namespace Applications.DTOs.Response
{
    public class FacilityImageResponseDto
    {
        public string ImageId { get; set; } = string.Empty;
        public string FacilityId { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime UploadDate { get; set; }
        public byte? ImageOrder { get; set; }
    }
}

