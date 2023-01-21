using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using API.Entities.OrderModel;

namespace API.DTOs.Order
{
    public class OrderRequest
    {
        public string ReceiverName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public ICollection<OrderItemRequest> OrderItemRequests { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public int ShippingMethod { get; set; }
        public bool IsFromCart { get; set; }
        public string CardHolder { get; set; }
        public string CardNumber { get; set; }
        public string ExpiredDate { get; set; }
        public int CVV { get; set; }
    }

    public class PayOrderRequest
    {
        public string CardHolder { get; set; }
        public string CardNumber { get; set; }
        public string ExpiredDate { get; set; }
        public int CVV { get; set; }
    }

    public class OrderItemRequest {
        public int OptionId { get; set; }
        public int Quantity { get; set; }
    }

    public class CancelOrderRequest {
        public string Reason { get; set; }
    }

     public class ReturnOrderRequest {
        public string Reason { get; set; }
    }
}