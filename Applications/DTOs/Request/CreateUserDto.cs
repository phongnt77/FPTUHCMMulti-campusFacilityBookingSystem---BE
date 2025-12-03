using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateUserDto
    {
        public string? Email { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }

        [Required]
        public string RoleId { get; set; } = string.Empty;

        [Required]
        public string CampusId { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }
    }
}


