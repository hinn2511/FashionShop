using API.Entities.WebPageModel;

namespace API.DTOs.Request.ContentRequest
{
    public class CarouselRequest
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

    public class AddCarouselRequest : CarouselRequest
    {
        
    }

    public class UpdateCarouselRequest : CarouselRequest
    {
        
    }

     public class DeleteCarouselsRequest : BaseBulkRequest
    {
        
    }

     public class HideCarouselsRequest : BaseBulkRequest
    {
        
    }


}