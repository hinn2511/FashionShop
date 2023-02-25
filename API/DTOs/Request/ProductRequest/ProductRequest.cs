using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.ProductModel;

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

    public class CreateProductSaleRequest : BaseBulkRequest
    {
        public ProductSaleOffType SaleOffType { get; set; }
        public double Value { get; set; }
        public double Percent { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

}