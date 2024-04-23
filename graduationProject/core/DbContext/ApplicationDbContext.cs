using graduationProject.DTOs.offers;
using graduationProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace graduationProject.core.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,IdentityRole<int>,int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<UserRating> userRating { get; set; }
        public DbSet<Users> users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<React> Reacts { get; set; }
        public DbSet<offer> Offers { get; set; }
    }
}
