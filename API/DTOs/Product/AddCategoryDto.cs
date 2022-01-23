using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Product
{
    public class AddCategoryDto
    {
        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}