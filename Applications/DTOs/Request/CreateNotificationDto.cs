using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class CreateNotificationDto
    {
        [Required(ErrorMessage = "ID người dùng là bắt buộc")]
        public string UserId { get; set; } = string.Empty;

        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
    }
}

