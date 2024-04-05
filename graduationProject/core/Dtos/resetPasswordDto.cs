using System.ComponentModel.DataAnnotations;

namespace graduationProject.core.Dtos
{
    public class resetPasswordDto
    {
        [Required]
        public string Password { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
