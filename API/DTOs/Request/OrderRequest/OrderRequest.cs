using System.Collections.Generic;

namespace API.DTOs.Order
{
    public class OrderRequest
    {
        public ICollection<OrderItemRequest> OrderItemRequests { get; set; }
        public bool IsFromCart { get; set; }
    }

    public class OrderItemRequest {
        public int ProductId { get; set; }
        public int OptionId { get; set; }
        public int Quantity { get; set; }
    }
}