using System;
using System.Collections.Generic;
using API.Entities;
using API.Entities.ProductModel;

namespace API.DTOs.Request.CategoryRequest
{
    public class CategoryRequest
    {
        public string CategoryName { get; set; }
        public Gender Gender { get; set; }
    }

    #region manager
    public class AdminCategoryRequest : CategoryRequest
    {
        public int Id { get; set; }
        public Status Status { get; set; }
    }

    public class CreateCategoryRequest : CategoryRequest
    {

    }

    public class UpdateCategoryRequest : CategoryRequest
    {

    }

    public class HideCategoriesRequest : BaseBulkRequest
    {
        public bool IncludeProducts { get; set; }
    }

    public class DeleteCategoriesRequest : BaseBulkRequest
    {
        public bool IncludeProducts { get; set; }
    }

    #endregion
}