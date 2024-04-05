using System.ComponentModel.DataAnnotations;

namespace graduationProject.DTOs
{
    public class ResetTokenDto
    {
        [Required]
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        

    }
}
