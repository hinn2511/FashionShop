using System.Text.Json.Serialization;
using API.Entities.ProductModel;

namespace API.Entities.WebPageModel
{
    public class FeatureProduct : BaseEntity
    {
        [JsonIgnore]
        public HomePage HomePage { get; set; }
        public int HomePageId { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public string Link { get; set; }
        
    }
}