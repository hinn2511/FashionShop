using API.Entities.ProductModel;

namespace API.Helpers
{
    public class BaseProductParams : PaginationParams
    {
        public string Query { get; set; }
        public string Category { get; set; }
        public Gender? Gender { get; set; }
    }

    public class CustomerProductParams : BaseProductParams
    {

    }

    public class AdministratorProductParams : BaseProductParams
    {
        public ProductStatus Status { get; set; }
    }
}