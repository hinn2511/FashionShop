using System.Collections.Generic;

namespace API.DTOs.Order
{
    public class OrderRequest
    {
        public ICollection<OrderItemRequest> OrderItemRequests { get; set; }
        public bool IsFromCart { get; set; }
    }

    public class OrderItemRequest {
        public int OptionId { get; set; }
        public int Quantity { get; set; }
    }

    public class CancelOrderRequest {
        public int OrderId { get; set; }
        public string Reason { get; set; }
    }
}