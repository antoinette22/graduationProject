using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace graduationProject.Models
{
    public class verificationCode
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        public DateTime CreatedAt{ get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsEpired => DateTime.UtcNow > ExpiresAt;
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }
}
