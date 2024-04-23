using graduationProject.core.DbContext;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace graduationProject.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string? Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsEpired => DateTime.UtcNow > ExpiresAt;
        public Users User {get ;set;}
    }
}
