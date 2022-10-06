using System.Collections.Generic;

namespace API.DTOs.Response.OptionResponse
{
    public class OptionResponse
    {
        public OptionColorResponse Color { get; set; }
        public List<OptionSizeResponse>  Sizes { get; set; }
    }

    public class OptionColorResponse
    {
        public int Id { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }

    public class OptionSizeResponse
    {
        public int Id { get; set; }
        public string SizeName { get; set; }
        public int OptionId { get; set; }
        public double AdditionalPrice { get; set; }
    }

   
}