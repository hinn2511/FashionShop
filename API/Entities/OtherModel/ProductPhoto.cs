using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using API.Entities.Other;
using API.Entities.OtherModel;
using API.Entities.ProductModel;
using static API.Data.Constant;

namespace API.Entities.Other
{
    public class ProductPhoto : Photo
    {
        public bool IsMain { get; set; }

        [JsonIgnore]
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public FileType FileType { get; set; }
    }

    
}