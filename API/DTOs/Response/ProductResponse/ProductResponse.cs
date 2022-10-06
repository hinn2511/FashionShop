using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.ProductModel;

namespace API.DTOs.Response
{
    #region base product response
    public class ProductResponse : BaseResponse
    {
        public string ProductName { get; set; }
        public string Slug { get; set; }        
        public double Price { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }

    #endregion

    #region customer product response
    public class CustomerProductsResponse : ProductResponse
    {
        public bool LikedByUser { get; set; }
    }

    public class CustomerProductDetailResponse : ProductResponse
    {
        public int Category { get; set; }
        public int? SubCategory { get; set; }
        public int Brand { get; set; }
        public ICollection<CustomerProductPhotoResponse> ProductPhotos { get; set; }
    }

    public class CustomerProductPhotoResponse : BasePhotoResponse
    {

    }

    #endregion

    #region admin product response

    public class AdminProductsResponse : ProductResponse
    {
        public ProductStatus Status { get; set; }
    }
    
    public class AdminProductDetailResponse : ProductResponse
    {
        public int CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int BrandId { get; set; }
        public ICollection<AdminProductPhotoResponse> ProductPhotos { get; set; }
    }

    public class AdminProductPhotoResponse : BasePhotoResponse
    {
        public bool IsHidden { get; set; }
    }

    #endregion
    
}