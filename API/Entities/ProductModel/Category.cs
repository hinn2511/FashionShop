using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Entities.ProductModel
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public string Slug { get; set; }
        public Gender Gender { get; set; }
        public ICollection<Product> Products { get; set; }
        public Category Parent { get; set; }
        public int? ParentId { get; set; }
        public ICollection<Category> SubCategories { get; set; }
        public string CategoryImageUrl { get; set; }
        public bool IsPromoted { get; set; }
    }

    public enum Gender 
    {
        Men, 
        Women,
        Unisex, 
        Kid
    }
}