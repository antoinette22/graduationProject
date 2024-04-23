namespace graduationProject.Models
{
    public class Reply
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public Comment Comment { get; set; }
        public string? Attachment { get; set; }
    }
}
