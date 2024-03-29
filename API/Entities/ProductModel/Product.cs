using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using API.Entities.Other;

namespace API.Entities.ProductModel
{
    public class Product : BaseEntity
    {
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public Brand Brand { get; set; }
        public int BrandId { get; set; }
        public double Price { get; set; }
        public int Sold { get; set; }
        public string Description { get; set; }
        
        [JsonIgnore]
        public ICollection<Option> Options { get; set; }
        public ICollection<ProductPhoto> ProductPhotos { get; set; }
        public bool IsPromoted { get; set; }
        public ProductSaleOffType SaleType { get; set; }
        public int SaleOffPercent { get; set; }
        public int SaleOffValue { get; set; }
        public DateTime SaleOffFrom { get; set; }
        public DateTime SaleOffTo { get; set; }
    }
    
    public enum ProductSaleOffType
    {
        None,
        SaleOffPercent,
        SaleOffValue
    }

}