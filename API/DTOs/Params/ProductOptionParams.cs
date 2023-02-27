using System.Collections.Generic;
using API.Entities;

namespace API.DTOs.Params
{
    public class AdminProductOptionParams : PaginationParams
    {
        public IList<int> ProductIds { get; set; }
        public IList<Status> ProductOptionStatus { get; set; } = new List<Status>() { Status.Active };
    }
}