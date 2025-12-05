using DAL.Models.Enums;

namespace DAL.Models
{
    public class User
    {
        public string UserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string RoleId { get; set; } = string.Empty;
        public string CampusId { get; set; } = string.Empty;
        public UserStatus Status { get; set; } = UserStatus.Active;
        public VerificationStatus IsVerify { get; set; } = VerificationStatus.Unverified;
        public string? AvatarUrl { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? EmailVerificationCode { get; set; }
        public DateTime? EmailVerificationCodeExpiry { get; set; }
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetCodeExpiry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Role Role { get; set; } = null!;
        public Campus Campus { get; set; } = null!;
        public ICollection<Campus> ManagedCampuses { get; set; } = new List<Campus>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Booking> ApprovedBookings { get; set; } = new List<Booking>();
        public ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();
        public ICollection<BookingFeedback> BookingFeedbacks { get; set; } = new List<BookingFeedback>();
        public ICollection<Facility> ManagedFacilities { get; set; } = new List<Facility>();
        public ICollection<FacilityMaintenance> AssignedMaintenances { get; set; } = new List<FacilityMaintenance>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Report> GeneratedReports { get; set; } = new List<Report>();
    }
}

