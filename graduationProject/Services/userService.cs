using graduationProject.core.DbContext;
using graduationProject.DTOs;
using graduationProject.Mapping;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task<List<GetOfferedUserDto>> getOfferedPosts(int id)
        {
            var user = await _context.users
                .Include(u => u.Posts)
                .ThenInclude(p => p.offers)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null || user.Posts == null)
                return null; // or handle the situation as appropriate

            // Filter the posts that have offers
            var postsWithOffers = user.Posts.Where(p => p.IsHaveOffer);

            var response = await postsWithOffers.MapToGetOfferedUserDtoMap();
            return response;
        }

        public async Task<ResultDto> RefuseOffer(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if(offer == null)
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message = "offer not found"
                };
            }   
            var post = _context.Posts.FirstOrDefault(o => o.Id==offer.PostId);
            post.IsHaveOffer = false;
            _context.Offers.Remove(offer);
            _context.Posts.Update(post);
            _context.SaveChanges();

            return new ResultDto()
            {
                IsSuccess = true,
                Message = "offer removed Successfuly"
            };


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

