using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using graduationProject.Models;

namespace graduationProject.core.DbContext
{
    public class UserRating
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Rate { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
