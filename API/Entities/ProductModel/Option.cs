using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.ProductModel
{
    public class Option : BaseEntity
    {
        public Product Product { get; set; }
        public Color Color { get; set; }
        public Size Size { get; set; }
        public int ProductId { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public double AdditionalPrice { get; set; }
    }
}