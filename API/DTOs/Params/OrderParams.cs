using System;
using API.Entities.OrderModel;

namespace API.DTOs.Params
{
    public class CustomerOrderParams : PaginationParams
    {
        public OrderStatus? OrderStatus { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class AdminOrderParams : PaginationParams
    {
        public OrderStatus? OrderStatus { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}