using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Product
{
    public class AddCategoryDto
    {
        public string CategoryName { get; set; }
        public string Gender { get; set; }
    }
}