using System.ComponentModel.DataAnnotations;

namespace Applications.DTOs.Request
{
    public class GoogleLoginDto
    {
        [Required(ErrorMessage = "IdToken is required")]
        public string IdToken { get; set; } = string.Empty;
    }
}


