using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.WebPageModel;

namespace API.DTOs.Request.ArticleRequest
{
    public class ArticleRequest
    {
        public string Headline { get; set; }
        public string Foreword { get; set; }
        public string Content { get; set; }
        public ContentType ContentType { get; set; }
        public string ThumbnailUrl { get; set; }
    }

    public class CreateArticleRequest : ArticleRequest
    {

    }

    public class UpdateArticleRequest : ArticleRequest
    {

    }

    public class DeleteArticlesRequest : BaseBulkRequest
    {

    }

    public class HideArticlesRequest : BaseBulkRequest
    {

    }

    public class DeleteArticlePhotosRequest : BaseBulkRequest
    {

    }

    public class HideArticlePhotosRequest : BaseBulkRequest
    {

    }

     public class EditorChoiceRequest : BaseBulkRequest
    {

    }

}