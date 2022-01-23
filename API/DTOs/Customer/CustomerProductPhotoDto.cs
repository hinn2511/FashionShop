using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Customer
{
    public class CustomerProductPhotoDto
    {
        public string Url { get; set; }
        public bool IsMain { get; set; }
    }
}