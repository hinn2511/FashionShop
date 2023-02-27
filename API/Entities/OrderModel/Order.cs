using System.Collections.Generic;
using API.Entities.User;

namespace API.Entities.OrderModel
{
    public class Order : BaseEntity
    {   public PaymentMethod PaymentMethod { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<OrderHistory> OrderHistories { get; set; }
        public OrderStatus CurrentStatus { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public string ReceiverName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ExternalId { get; set; }
        public double SubTotal { get; set; }
        public double Tax { get; set; }
        public string ShippingMethod { get; set; }
        public double ShippingFee { get; set; }
    }

    public class OrderHistory : BaseEntity
    {
        public Order Order { get; set; } 
        public int OrderId { get; set; }
        public string HistoryDescription { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }

    public class ShippingMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Fee { get; set; }
        public int MinDate { get; set; }
        public int MaxDate { get; set; }
    }

    public enum OrderStatus
    {
        Created,
        Checking,
        Paid,
        Processing,
        CancelRequested,
        Cancelled,
        Shipping,
        Shipped,
        Declined, 
        Finished,
        ReturnRequested,
        Returned        
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        CashOnDelivery,
        MobilePayment
    }

}