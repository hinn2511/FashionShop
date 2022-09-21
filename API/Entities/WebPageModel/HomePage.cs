using System.Collections.Generic;

namespace API.Entities.WebPageModel
{
    public class HomePage : BaseEntity
    {
        public bool IsActive { get; set; }
        public bool ShowCarousels { get; set; }
        public bool ShowFeatureCategories { get; set; }
        public bool ShowFeatureProducts { get; set; }
        public ICollection<Carousel> Carousels { get; set; }
        public ICollection<FeatureCategory> FeatureCategories { get; set; }
        public ICollection<FeatureProduct> FeatureProducts { get; set; }
    }
}