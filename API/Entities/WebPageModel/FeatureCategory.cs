using System.Text.Json.Serialization;
using API.Entities.ProductModel;

namespace API.Entities.WebPageModel
{
    public class FeatureCategory : BaseEntity
    {
        [JsonIgnore]
        public HomePage HomePage { get; set; }
        public int HomePageId { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
    }
}