using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
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
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public bool LikedByUser { get; set; }
        public IList<CustomerProductPhotoResponse> ProductPhotos { get; set; }
    }

    public class CustomerProductPhotoResponse : BasePhotoResponse
    {

    }

    #endregion

    #region admin product response

    public class AdminProductsResponse : ProductResponse
    {
        public Status Status { get; set; }
    }
    
    public class AdminProductDetailResponse : ProductResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public IList<AdminProductPhotoResponse> ProductPhotos { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedByUserId { get; set; }
        public DateTime DateDeleted { get; set; }
        public int DeletedByUserId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHidden { get; set; }
        public DateTime DateHidden { get; set; }
        public int HiddenByUserId { get; set; }
        public Status Status { get; set; }
    }

    public class AdminProductPhotoResponse : BasePhotoResponse
    {
        public Status Status { get; set; }
        public bool IsMain { get; set; }
    }

    #endregion
    
}