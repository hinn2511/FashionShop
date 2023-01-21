using System;
using System.Collections.Generic;
using API.Entities;
using API.Entities.ProductModel;

namespace API.DTOs.Response.CategoryResponse
{
    public class CategoryResponse
    {
        public string Slug { get; set; }
        public string CategoryName { get; set; }
        public int ParentId { get; set; }
        public Gender Gender { get; set; }
        public string CategoryImageUrl { get; set; }
    }

    #region customer

      public class CustomerCategoryByGenderResponse
    {
        public Gender Gender { get; set; }
        public string GenderTitle { get; set; }
        public List<CustomerCategoryResponse> Categories { get; set; }
    }


    public class CustomerCategoryResponse : CategoryResponse
    {
        public List<CustomerCategoryResponse> SubCategories { get; set; }
    }


    #endregion

    #region manager
    public class AdminCategoryResponse : CategoryResponse
    {
        public string GenderName { get; set; }
        public Status Status { get; set; }
        public int Id { get; set; }
        public string ParentCategory { get; set; }
        public bool IsPromoted { get; set; }
    }

    public class AdminCategoryDetailResponse : CategoryResponse
    {
        public int Id { get; set; }
        public Status Status { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedByUserId { get; set; }
        public DateTime DateDeleted { get; set; }
        public int DeletedByUserId { get; set; }
        public DateTime DateHidden { get; set; }
        public int HiddenByUserId { get; set; }
        public List<AdminSubCategoryResponse> SubCategories { get; set; }
    }

    public class AdminSubCategoryResponse
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public Status Status { get; set; }
    }

    public class AdminCatalogueResponse
    {
        public Gender Gender { get; set; }
        public string GenderTitle { get; set; }
        public List<AdminCatalogueCategoryResponse> Categories { get; set; }
    }


    public class AdminCatalogueCategoryResponse : CategoryResponse
    {
        public int Id { get; set; }
        public List<AdminCatalogueCategoryResponse> SubCategories { get; set; }
    }

    #endregion
}