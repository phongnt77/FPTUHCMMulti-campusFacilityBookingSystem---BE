using DAL.Models.Enums;

namespace DAL.Models
{
    public class Campus
    {
        public string CampusId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? FacilityManagerId { get; set; }
        public CampusStatus Status { get; set; } = CampusStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User? FacilityManager { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}

