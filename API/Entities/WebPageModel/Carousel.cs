using System.Text.Json.Serialization;

namespace API.Entities.WebPageModel
{
    public class Carousel : BaseEntity
    {
        [JsonIgnore]
        public HomePage HomePage { get; set; }
        public int HomePageId { get; set; }
        public string ImageUrl { get; set; }
        public string Text { get; set; }
        public string TextLink { get; set; }
        public TextPosition TextPosition { get; set; }
        public int TextPaddingLeft { get; set; }
        public int TextPaddingRight { get; set; }
        public int TextPaddingTop { get; set; }
        public int TextPaddingBottom { get; set; }
        
    }
}