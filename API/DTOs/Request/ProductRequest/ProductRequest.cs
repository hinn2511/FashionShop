using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Request.ProductRequest
{
    public class ProductRequest
    {
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int BrandId { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
    }

    public class CreateProductRequest : ProductRequest
    {

    }

    public class UpdateProductRequest : ProductRequest
    {

    }

    public class DeleteProductsRequest : BaseBulkRequest
    {

    }

    public class HideProductsRequest : BaseBulkRequest
    {

    }

    public class DeleteProductPhotosRequest : BaseBulkRequest
    {

    }

    public class HideProductPhotosRequest : BaseBulkRequest
    {

    }

}