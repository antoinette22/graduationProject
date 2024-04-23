using graduationProject.core.DbContext;
using graduationProject.Models;
using graduationProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace graduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Investor")]
    public class offerController : ControllerBase
    {
       private readonly IOfferService _offerService;

        public offerController(IOfferService offerService)
        {
            _offerService = offerService; 
        }

        [HttpPost("send offer")]

        public async Task<IActionResult> sendOffer(int postId, string offerContent, IFormFile image)

        {
            var result = await _offerService.sendOfferToPost( postId,  offerContent, image); 
            return Ok(result);
        }

    }
}
