using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Customer
{
    public class CustomerProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public string Gender { get; set; }
        public double Price { get; set; }
        public int Sold { get; set; }
        public ICollection<CustomerProductStockDto> Stocks { get; set; }
        public ICollection<CustomerProductPhotoDto> ProductPhotos { get; set; }
    }
}