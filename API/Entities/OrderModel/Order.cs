using System.Collections.Generic;

namespace API.Entities.OrderModel
{
    public class Order : BaseEntity
    {   public PaymentMethod PaymentMethod { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<OrderHistory> OrderHistories { get; set; }
        public OrderStatus CurrentStatus { get; set; }
    }

    public class OrderHistory : BaseEntity
    {
        public Order Order { get; set; } 
        public int OrderId { get; set; }
        public string Note { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }

    public enum OrderStatus
    {
        AwaitingPayment,
        Paid,
        Processing,
        Shipping,
        Shipped,
        Finished,
        Declined, 
        RefundRequested,
        RefundAccepted,
        RefundedAndReturn,
        Returned,
        RefundedAndNoReturn,
        CancelRequested,
        Cancelled,
    }

    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        DebitCard
    }

}