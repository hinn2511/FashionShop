using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Entities.ProductEntities
{
    public class ProductPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        
        [JsonIgnore]
        public Product Product { get; set; }
        public int ProductId { get; set; }

    }
}