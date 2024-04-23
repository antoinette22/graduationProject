using graduationProject.DTOs;

namespace graduationProject.Services
{
    public interface IOfferService
    {
        Task<ResultDto> sendOfferToPost(int postId, string offerContent, IFormFile image);
    }
}
