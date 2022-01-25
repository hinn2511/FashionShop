using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Product
{
    public class ProductPhotoDto
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
    }
}