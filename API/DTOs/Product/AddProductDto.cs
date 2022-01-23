using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Product
{
    public class AddProductDto
    {
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public double ProductPrice { get; set; }
    }
}