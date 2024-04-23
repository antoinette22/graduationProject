using graduationProject.Models;

namespace graduationProject.DTOs.offers
{
    public class offer
    {
        public int Id {  get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public string OfferContent { get; set; }
        public string Image { get; set; }


    }
}
