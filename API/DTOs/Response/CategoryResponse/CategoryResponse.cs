using System;
using System.Collections.Generic;
using API.Entities;
using API.Entities.ProductModel;

namespace API.DTOs.Response.CategoryResponse
{
    public class CategoryResponse
    {
        public string CategoryName { get; set; }
        public Gender Gender { get; set; }
    }

    #region customer

    public class CustomerCategoryResponse : CategoryResponse
    {
        public List<CustomerSubCategoryResponse> SubCategories { get; set; }
    }

    public class CustomerSubCategoryResponse
    {
        public string CategoryName { get; set; }
    }
    #endregion

    #region manager
    public class AdminCategoryResponse : CategoryResponse
    {
        public Status Status { get; set; }
        public int Id { get; set; }
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
    #endregion
}