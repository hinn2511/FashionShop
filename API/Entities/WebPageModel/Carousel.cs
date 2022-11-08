using System.Text.Json.Serialization;

namespace API.Entities.WebPageModel
{
    public class Carousel : BaseEntity
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string NavigationText { get; set; }
        public string Link { get; set; }
        public TextPosition TextPosition { get; set; }
        public int TextPaddingLeft { get; set; }
        public int TextPaddingRight { get; set; }
        public int TextPaddingTop { get; set; }
        public int TextPaddingBottom { get; set; }
        
    }
}