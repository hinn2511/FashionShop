using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.ProductEntities
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public double ProductPrice { get; set; }
        public DateTime CreateAt { get; set; }
        public int Sold { get; set; }
        public ICollection<Stock> Stocks { get; set; }
        public ICollection<ProductPhoto> ProductPhotos { get; set; }
    }
}