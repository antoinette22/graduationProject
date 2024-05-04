using graduationProject.core.DbContext;
using graduationProject.DTOs.offers;
using Microsoft.AspNetCore.Identity;

namespace graduationProject.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
       // public ApplicationUser User { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<React> Reacts { get; set; }
        public string? Attachment { get; set; }
        public IEnumerable<offer> offers { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }

    }
}
