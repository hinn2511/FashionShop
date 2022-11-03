using System.Collections.Generic;

namespace API.Entities.ProductModel
{
    public class SubCategory : BaseEntity
    {
        public Category Category { get; set; }
        public string Slug { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}