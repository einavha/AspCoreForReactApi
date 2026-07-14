namespace AspCoreForReactApi.Controllers
{
    public class ArticleItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Excerpt { get; set; }        
        public string? Content {get; set;}
        public string? FeaturedImage { get; set; }
    }
}

