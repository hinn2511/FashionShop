namespace API.DTOs.Request.SizeRequest
{
     public class SizeRequest
    {
        public string SizeName { get; set; }
    }
    
    public class CreateSizeRequest : SizeRequest
    {
    }

    public class UpdateSizeRequest : SizeRequest
    {
    }
}