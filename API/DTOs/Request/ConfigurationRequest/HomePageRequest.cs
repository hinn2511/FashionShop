using System.Collections.Generic;

namespace API.DTOs.Request.ConfigurationRequest
{
    public class HomePageRequest
    {
        public ICollection<CarouselRequest> Carousels { get; set; }
        public ICollection<FeatureCategoryRequest> FeatureCategories { get; set; }
        public ICollection<FeatureProductRequest> FeatureProducts { get; set; }

        public bool ShowCarousels { get; set; }
        public bool ShowFeatureCategories { get; set; }
        public bool ShowFeatureProducts { get; set; }
    }
}