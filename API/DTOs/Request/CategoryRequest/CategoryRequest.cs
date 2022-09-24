using API.Entities.ProductModel;

namespace API.DTOs.Request.CategoryRequest
{
    public class CategoryRequest
    {
        public string CategoryName { get; set; }
        public Gender Gender { get; set; }
    }
}