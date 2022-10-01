using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Response
{
    public class ProductResponse : BaseResponse
    {
        public string ProductName { get; set; }
        public string Slug { get; set; }        
        public int CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int BrandId { get; set; }
        public double Price { get; set; }
        public int Sold { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public ICollection<ProductPhotoResponse> ProductPhotos { get; set; }
    }

    public class ProductPhotoResponse : BasePhotoResponse
    {
        public bool IsMain { get; set; }
    }

    public class CustomerGetAllProductResponse : ProductResponse
    {

    }
}