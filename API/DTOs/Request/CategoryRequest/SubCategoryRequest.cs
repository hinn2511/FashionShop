namespace API.DTOs.Request.CategoryRequest
{
    public class SubCategoryRequest : CategoryRequest
    {
        public int ParentCategoryId { get; set; }
    }

    public class UpdateSubCategoryRequest : SubCategoryRequest
    {
        public int Id { get; set; }
    }
}