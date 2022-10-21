using API.Entities.ProductModel;

namespace API.DTOs.Request.CategoryRequest
{
    public class SubCategoryRequest
    {
        public string CategoryName { get; set; }
        public Gender Gender { get; set; }
    }

    public class UpdateSubCategoryRequest : SubCategoryRequest
    {
        public int Id { get; set; }
    }
}