using API.Entities.ProductModel;

namespace API.Helpers
{
    public class ProductParams : PaginationParams
    {
        public string Category { get; set; }
        public Gender? Gender { get; set; }
    }
}