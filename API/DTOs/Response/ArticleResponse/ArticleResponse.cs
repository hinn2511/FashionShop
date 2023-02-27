using API.Entities;
using API.Entities.WebPageModel;

namespace API.DTOs.Response.ArticleResponse
{
    public class BaseArticleResponse
    {
        public int Id { get; set; }
        public string Headline { get; set; }
        public string HeadlineSlug { get; set; }
        public string Foreword { get; set; }
        public string PublishedBy { get; set; }
        public string PublishedDate { get; set; }
        public string ThumbnailUrl { get; set; }
        public int View { get; set; }
         public ContentType ContentType { get; set; }
        public bool EditorChoice { get; set; }
    }

    public class BaseArticleDetailResponse
    {
        public int Id { get; set; }
        public string Headline { get; set; }
        public string HeadlineSlug { get; set; }
        public string Foreword { get; set; }
        public string Content { get; set; }
        public ContentType ContentType { get; set; }
        public string PublishedBy { get; set; }
        public string PublishedDate { get; set; }
        public string ThumbnailUrl { get; set; }
         public int View { get; set; }
         public bool EditorChoice { get; set; }
    }

    public class CustomerArticleResponse : BaseArticleResponse
    {
    }

    public class CustomerArticleDetailResponse : BaseArticleDetailResponse
    {
        
    }

    public class AdminArticleResponse : BaseArticleResponse
    {
        public Status Status { get; set; }
    }

    public class AdminArticleDetailResponse : BaseArticleDetailResponse
    {
        public Status Status { get; set; }
    }
}