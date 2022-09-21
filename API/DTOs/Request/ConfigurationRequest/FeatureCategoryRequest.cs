using API.Entities.ProductModel;

namespace API.DTOs.Request.ConfigurationRequest
{
    public class FeatureCategoryRequest
    {
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryUrl { get; set; }
    }
}