namespace graduationProject.Dtos
{
    public class AddPostDto
    {
        public string Content { get; set; }
        public IFormFile? Attachment { get; set; }
    }
}
