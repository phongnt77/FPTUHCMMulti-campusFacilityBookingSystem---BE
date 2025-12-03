namespace Applications.DTOs.Request
{
    public class FacilityFilterDto : PagedRequestDto
    {
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? TypeId { get; set; }
        public string? CampusId { get; set; }
    }
}


