using graduationProject.core.DbContext;
using graduationProject.DTOs.offers;
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

        public async Task<IActionResult> SendOfferAsync([FromForm] offerDto offer)

        {
            var result = await _offerService.sendOfferToPost(offer);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("something went wrong");

        }
        // Web API Controller
        //[Route("api/images")]


        //    [HttpGet("offer")]
        //    public IActionResult GetOfferImage()
        //    {
        //        // Read the image file from the wwwroot/images folder
        //        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "offer_image.jpg");

        //        // Check if the file exists
        //        if (!System.IO.File.Exists(imagePath))
        //            return NotFound();

        //        // Return the image file
        //        var imageData = System.IO.File.ReadAllBytes(imagePath);
        //        return File(imageData, "image/jpeg");
        //    }



    }
}
