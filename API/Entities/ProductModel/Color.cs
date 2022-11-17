using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.ProductModel
{
    public class Color : BaseEntity
    {
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }
}