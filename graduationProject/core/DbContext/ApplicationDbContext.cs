using graduationProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace graduationProject.core.DbContext
{
    public class ApplicationDbContext : IdentityDbContext/*<verificationCode>*/
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<verificationCode> verificationCodes { get; set; }
        public DbSet<UserRating> userRating { get; set; }
    }
}
