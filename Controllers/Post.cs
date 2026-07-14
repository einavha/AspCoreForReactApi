namespace AspCoreForReactApi.Controllers
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Excerpt { get; set; }
        public string? Content { get; set; }
        public string? FeaturedImage { get; set; }
        public List<Attachment>? Attachments { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
    }

    public class Attachment
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public string? ContentType { get; set; }
    }
}