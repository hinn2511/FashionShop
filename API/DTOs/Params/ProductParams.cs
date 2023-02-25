using System.Collections.Generic;
using API.Entities;
using API.Entities.ProductModel;

namespace API.DTOs.Params
{
    public class BaseProductParams : PaginationParams
    {
        public string Category { get; set; }
        public Gender? Gender { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        
    }

    public class CustomerProductParams : BaseProductParams
    {
        
        public string Size { get; set; }
        public string ColorCode { get; set; }
        public bool IsOnSale { get; set; }
    }

    public class AdministratorProductParams : BaseProductParams
    {
        public IList<Status> ProductStatus { get; set; } = new List<Status>() { Status.Active };
    }
}