using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class ProductDto
    {
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public String Category { get; set; }
    }
}