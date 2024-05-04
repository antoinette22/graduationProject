using graduationProject.DTOs;
using graduationProject.DTOs.offers;

namespace graduationProject.Services
{
    public interface IOfferService
    {
        Task<ResultDto> sendOfferToPost(offerDto Offer);
    }
}
