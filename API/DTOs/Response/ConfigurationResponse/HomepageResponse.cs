using System.Collections.Generic;
using API.DTOs.Response.ContentResponse;
using API.Entities.WebPageModel;

namespace API.DTOs.Response.ConfigurationResponse
{
    #region base homepage
    public class HomePageResponse
    {
    }

    public class FeatureProductResponse
    {
        public string ImageUrl { get; set; }
        public string ProductUrl { get; set; }
        public string ProductName { get; set; }
    }

    public class FeatureCategoryResponse
    {
        public string ImageUrl { get; set; }
        public string CategoryUrl { get; set; }
        public string CategoryName { get; set; }
    }

    public class CarouselResponse
    {
        public string ImageUrl { get; set; }
        public string Text { get; set; }
        public string TextLink { get; set; }
        public TextPosition TextPosition { get; set; }
        public int TextPaddingLeft { get; set; }
        public int TextPaddingRight { get; set; }
        public int TextPaddingTop { get; set; }
        public int TextPaddingBottom { get; set; }
    }

    #endregion

    public class CustomerHomePageResponse : HomePageResponse
    {
        public List<CustomerCarouselResponse> Carousels { get; set; }

        public List<CustomerFeatureCategoryResponse> FeatureCategories { get; set; }

        public List<CustomerFeatureProductResponse> FeatureProducts { get; set; }
    }

    public class CustomerFeatureProductResponse :  FeatureProductResponse
    {
    }

    public class CustomerFeatureCategoryResponse : FeatureCategoryResponse
    {
    }

   
}