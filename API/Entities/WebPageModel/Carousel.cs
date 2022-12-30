using System.Text.Json.Serialization;

namespace API.Entities.WebPageModel
{
    public class Carousel : BaseEntity
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        
    }
}