using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class UpdateUserProfileDto
    {
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải có 10 chữ số và bắt đầu bằng 0.")]
        public string? PhoneNumber { get; set; }

        public string? AvatarUrl { get; set; }
    }
}



