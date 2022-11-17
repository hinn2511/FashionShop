namespace API.DTOs.Request.ColorRequest
{
    public class ColorRequest
    {
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }
    
    public class CreateColorRequest : ColorRequest
    {
    }

    public class UpdateColorRequest : ColorRequest
    {
    }
}