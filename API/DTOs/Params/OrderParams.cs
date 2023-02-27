using System;
using System.Collections.Generic;
using API.Entities.OrderModel;

namespace API.DTOs.Params
{
    public class CustomerOrderParams : PaginationParams
    {
        public IList<OrderStatus> OrderStatusFilter { get; set; } = new List<OrderStatus>() { OrderStatus.Created };
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class AdminOrderParams : PaginationParams
    {
        public IList<OrderStatus> OrderStatusFilter { get; set; } = new List<OrderStatus>() {};
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IList<PaymentMethod> PaymentMethodFilter { get; set; } = new List<PaymentMethod>() {};
        public IList<string> ShippingMethodFilter { get; set; } = new List<string>() {};
    }
}