namespace API.Entities.WebPageModel
{
    public class News : BaseEntity
    {
        public string Headline { get; set; }
        public string HeadlineSlug { get; set; }
        public string Foreword { get; set; }
        public string Content { get; set; }
        public string ContentAbstract { get; set; }
        public ContentType ContentType { get; set; }
    }

    public enum ContentType {
        Promotion,
        News,
    }
}