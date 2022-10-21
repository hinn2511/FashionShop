using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using API.Entities.OrderModel;

namespace API.DTOs.Order
{
    public class OrderRequest
    {
        [Required]
        public ICollection<OrderItemRequest> OrderItemRequests { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsFromCart { get; set; }
    }

    public class OrderItemRequest {
        public int OptionId { get; set; }
        public int Quantity { get; set; }
    }

    public class PayOrderRequest
    {
        public string CardNumber { get; set; }
        public DateTime ExpiredDate { get; set; }
        public int CVV { get; set; }
    }

    public class CancelOrderRequest {
        public string Reason { get; set; }
    }
}