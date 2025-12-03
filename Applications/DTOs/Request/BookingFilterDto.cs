namespace Applications.DTOs.Request
{
    public class BookingFilterDto : PagedRequestDto
    {
        public string? UserId { get; set; }
        public string? FacilityId { get; set; }
        public string? Status { get; set; }
    }
}

