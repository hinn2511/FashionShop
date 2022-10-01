using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using API.Entities.Other;
using API.Entities.ProductModel;

namespace API.Entities.Other
{
    public class ProductPhoto : Photo
    {
        public bool IsMain { get; set; }
        
        [JsonIgnore]
        public Product Product { get; set; }
        public int ProductId { get; set; }

    }
}