using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateRoleDto
    {
        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; } = string.Empty;
    }
}

