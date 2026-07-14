namespace AspCoreForReactApi.Controllers
{
    public class WordPressPost
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateGmt { get; set; }
        public Guid Guid { get; set; }
        public DateTime Modified { get; set; }
        public DateTime ModifiedGmt { get; set; }
        public string Slug { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Link { get; set; }
        public Title Title { get; set; }
        public Content Content { get; set; }
        public Excerpt Excerpt { get; set; }
        public int Author { get; set; }
        public int FeaturedMedia { get; set; }
        public string CommentStatus { get; set; }
        public string PingStatus { get; set; }
        public bool Sticky { get; set; }
        public string Template { get; set; }
        public string Format { get; set; }
        public Meta Meta { get; set; }
        public List<int> Categories { get; set; }
        public List<object> Tags { get; set; }
        public List<string> ClassList { get; set; }
        public Links Links { get; set; }
    }

    public class About
    {
        public string Href { get; set; }
    }

    public class Author
    {
        public bool Embeddable { get; set; }
        public string Href { get; set; }
    }

    public class Collection
    {
        public string Href { get; set; }
    }

    public class Content
    {
        public string Rendered { get; set; }
        public bool Protected { get; set; }
    }

    public class Cury
    {
        public string Name { get; set; }
        public string Href { get; set; }
        public bool Templated { get; set; }
    }

    public class Excerpt
    {
        public string Rendered { get; set; }
        public bool Protected { get; set; }
    }

    public class Guid
    {
        public string Rendered { get; set; }
    }

    public class Links
    {
        public List<Self> Self { get; set; }
        public List<Collection> Collection { get; set; }
        public List<About> About { get; set; }
        public List<Author> Author { get; set; }
        public List<Reply> Replies { get; set; }

        public List<VersionHistory> VersionHistory { get; set; }

        public List<WpAttachment> WpAttachment { get; set; }

        public List<WpTerm> WpTerm { get; set; }
        public List<Cury> Curies { get; set; }
    }

    public class Meta
    {
        public string Footnotes { get; set; }
    }

    public class Reply
    {
        public bool Embeddable { get; set; }
        public string Href { get; set; }
    }


    public class Self
    {
        public string Href { get; set; }
        public TargetHints TargetHints { get; set; }
    }

    public class TargetHints
    {
        public List<string> Allow { get; set; }
    }

    public class Title
    {
        public string Rendered { get; set; }
    }

    public class VersionHistory
    {
        public int Count { get; set; }
        public string Href { get; set; }
    }

    public class WpAttachment
    {
        public string Href { get; set; }
    }

    public class WpTerm
    {
        public string Taxonomy { get; set; }
        public bool Embeddable { get; set; }
        public string Href { get; set; }
    }
}

