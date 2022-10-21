using System.Text.Json.Serialization;

namespace API.Entities.WebPageModel
{
    public class Carousel : BaseEntity
    {
        [JsonIgnore]
        public HomePage HomePage { get; set; }
        public int HomePageId { get; set; }
        public string ImageUrl { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public string ButtonText { get; set; }
        public string Link { get; set; }
        public TextPosition TextPosition { get; set; }
        public int TextPaddingLeft { get; set; }
        public int TextPaddingRight { get; set; }
        public int TextPaddingTop { get; set; }
        public int TextPaddingBottom { get; set; }
        
    }
}