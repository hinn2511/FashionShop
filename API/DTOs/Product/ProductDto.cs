using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public string Category { get; set; }
        public double ProductPrice { get; set; }
        public DateTime CreateAt { get; set; }
        public int Sold { get; set; }
    }
}