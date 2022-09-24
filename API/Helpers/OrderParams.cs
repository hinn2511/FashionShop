using System;
using API.Entities.OrderModel;

namespace API.Helpers
{
    public class OrderParams : PaginationParams
    {
        public OrderStatus? OrderStatus { get; set; }
        public OrderBy OrderBy { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int? Id { get; set; }
        public int? CreatedByUserId { get; set; }
    }
}