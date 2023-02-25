using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.ProductModel
{
    public class Option : BaseEntity
    {
        public Product Product { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public string SizeName { get; set; }
        public int ProductId { get; set; }
        public double AdditionalPrice { get; set; }
    }
}