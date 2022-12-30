using API.Entities.Other;
using API.Entities.User;

namespace API.Entities.WebPageModel
{
    public class Article : BaseEntity
    {
        public string Headline { get; set; }
        public string HeadlineSlug { get; set; }
        public string Foreword { get; set; }
        public string Content { get; set; }
        public string ContentAbstract { get; set; }
        public ContentType ContentType { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public string ThumbnailUrl { get; set; }
        public int View { get; set; }
        public bool EditorChoice { get; set; }
    }

    public enum ContentType {
        Promotion,
        Blog,
        Information, 
        Announcement
    }
}