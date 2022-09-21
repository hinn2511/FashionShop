using System;
using API.Entities.OrderModel;

namespace API.DTOs.Request.OrderRequest
{
    public class PayRequest
    {
        public int OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public string CardNumber { get; set; }
        public DateTime ExpiredDate { get; set; }
        public int CVV { get; set; }
    }
}