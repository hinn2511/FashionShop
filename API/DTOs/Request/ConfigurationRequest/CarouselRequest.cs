using API.Entities.WebPageModel;

namespace API.DTOs.Request.ConfigurationRequest
{
    public class CarouselRequest
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
}