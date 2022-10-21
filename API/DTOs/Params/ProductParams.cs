using System.Collections.Generic;
using API.Entities;
using API.Entities.ProductModel;

namespace API.DTOs.Params
{
    public class BaseProductParams : PaginationParams
    {
        public string Category { get; set; }
        public Gender? Gender { get; set; }
        public double FromPrice { get; set; }
        public double ToPrice { get; set; }
    }

    public class CustomerProductParams : BaseProductParams
    {
        
    }

    public class AdministratorProductParams : BaseProductParams
    {
        public IList<Status> ProductStatus { get; set; } = new List<Status>() { Status.Active };
    }
}