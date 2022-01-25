using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Customer
{
    public class CustomerCategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Gender { get; set; }
    }
}