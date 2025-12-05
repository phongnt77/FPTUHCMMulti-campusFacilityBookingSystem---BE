using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateUserDto
    {
        public string? Email { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        public string RoleId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cơ sở là bắt buộc")]
        public string CampusId { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }
    }
}


