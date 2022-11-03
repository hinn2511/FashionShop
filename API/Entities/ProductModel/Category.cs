using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.ProductModel
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public string Slug { get; set; }
        public Gender Gender { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<SubCategory> SubCategories { get; set; }
    }

    public enum Gender 
    {
        Men, 
        Women,
        Unisex, 
        Kid
    }
}