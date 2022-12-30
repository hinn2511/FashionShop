using System.Collections.Generic;
using API.Entities;
using API.Entities.WebPageModel;

namespace API.DTOs.Params
{
    public class CustomerArticleParams : PaginationParams
    {
        public IList<ContentType> ContentTypes { get; set; } = new List<ContentType>() { ContentType.Blog, ContentType.Promotion };
    }

    public class AdminArticleParams : PaginationParams
    {
        public IList<ContentType> ContentTypes { get; set; } = new List<ContentType>() { ContentType.Blog };

        public IList<Status> ArticleStatus { get; set; } = new List<Status>() { Status.Active };
    }
}