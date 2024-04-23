using graduationProject.core.DbContext;
using graduationProject.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace graduationProject.Services
{
    public class userService : IuserService
    {
        private readonly ApplicationDbContext _context;
        public userService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<searchDto> SearchUserProfile(string userName)
        {
            var user = await _context.users.FirstOrDefaultAsync(a=>a.Name.Contains(userName.Trim())) ;
            if (user != null)
            {
                return new searchDto

                {
                    userName = user.Name,
                };
            }
           else return new searchDto

           {
               userName = null,
           };

        }
    }
}

