using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Customer
{
    public class CustomerProductStockDto
    {
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public string SizeName { get; set; }
        public int Quantity { get; set; }
    }
}