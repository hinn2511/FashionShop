using System;
using System.Collections.Generic;
using API.Entities.OrderModel;

namespace API.DTOs.Response.OrderResponse
{
    #region base response
    public class BaseOrderSummaryResponse
    {
        public string ExternalId { get; set; }

        public DateTime DateCreated { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public OrderStatus CurrentStatus { get; set; }

        public string PaymentMethodString { get; set; }
        
        public string CurrentStatusString { get; set; }

        public int TotalItem { get; set; }

        public double TotalPrice { get; set; }
    
    }

    public class BaseOrderResponse : BaseOrderSummaryResponse
    {
        public string ReceiverName { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public double SubTotal { get; set; }

        public double Tax { get; set; }

        public string ShippingMethod { get; set; }

        public double ShippingFee { get; set; }
        
       

    
    }

    public class BaseOrderDetailItemResponse
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public double Price { get; set; }

        public string Url { get; set; }

        public string ColorName { get; set; }

        public string ColorCode { get; set; }

        public string SizeName { get; set; }

        public int Quantity { get; set; }

        public double Total { get; set; }
        
    }

    public class BaseOrderHistoriesResponse
    {
        public DateTime DateCreated { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }


    #endregion


    #region customer
    public class CustomerOrderSummaryResponse : BaseOrderSummaryResponse
    {
        public ICollection<CustomerOrderItemResponse> OrderDetails { get; set; }
    }

    public class CustomerOrderResponse : BaseOrderResponse
    {
        public bool IsFinished { get; set; }
        public ICollection<CustomerOrderHistoriesResponse> OrderHistories { get; set; }
        public ICollection<CustomerOrderItemResponse> OrderDetails { get; set; }
    }

    public class CustomerOrderItemResponse : BaseOrderDetailItemResponse
    {
        
    }

    public class CustomerOrderHistoriesResponse : BaseOrderHistoriesResponse
    {
        public string Note { get; set; }
    }

    #endregion


    #region admin   
    public class AdminOrderCountResponse
    {
        public OrderStatus OrderStatus { get; set; }
        public int Total { get; set; }
    }


    public class AdminOrderSummaryResponse : BaseOrderSummaryResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }  
        public string ShippingMethod { get; set; }    
    }

    public class AdminOrderResponse : BaseOrderResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }  
        public int CreatedByUserId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedByUserId { get; set; }
        public DateTime DateDeleted { get; set; }
        public int DeletedByUserId { get; set; }
        public DateTime DateHidden { get; set; }
        public int HiddenByUserId { get; set; }
        public int Status { get; set; }
        public ICollection<AdminOrderHistoriesResponse> OrderHistories { get; set; }
        public ICollection<AdminOrderItemResponse> OrderDetails { get; set; }
    }

    public class AdminOrderItemResponse : BaseOrderDetailItemResponse
    {
        public int Id { get; set; }
        public int StockAvailable { get; set; }
        public int OptionId { get; set; }
        public int StockAfterDeduction { get; set; }
    }

    public class AdminOrderHistoriesResponse : BaseOrderHistoriesResponse
    {
        public int Id { get; set; }
        public string HistoryDescription { get; set; }
    }

    #endregion

}