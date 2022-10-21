using System;
using System.Collections.Generic;
using API.Entities.OrderModel;

namespace API.DTOs.Response.OrderResponse
{
    #region base response
    public class BaseOrderResponse : BaseResponse
    {
        public string ExternalId { get; set; }
        public DateTime DateCreated { get; set; }
        public int CurrentStatus { get; set; }
        public int TotalItem { get; set; }
        public double TotalPrice { get; set; }
        public string Url { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }

    public class BaseOrderDetailItemResponse
    {
        public int Id { get; set; }
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
    public class CustomerOrderResponse : BaseOrderResponse
    {
        
    }

    public class CustomerOrderDetailResponse : BaseOrderResponse
    {
        public ICollection<CustomerOrderHistoriesResponse> OrderHistories { get; set; }
        public ICollection<CustomerOrderDetailItemResponse> OrderDetails { get; set; }
    }

    public class CustomerOrderDetailItemResponse : BaseOrderDetailItemResponse
    {
    }

    public class CustomerOrderHistoriesResponse : BaseOrderHistoriesResponse
    {
        public string Note { get; set; }
    }

    #endregion


    #region admin   
    public class AdminOrderResponse : BaseOrderResponse
    {
        public int CreatedByUserId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedByUserId { get; set; }
        public DateTime DateDeleted { get; set; }
        public int DeletedByUserId { get; set; }
        public DateTime DateHidden { get; set; }
        public int HiddenByUserId { get; set; }
        public int Status { get; set; }
    }

    public class AdminOrderDetailResponse : BaseOrderResponse
    {
        public int CreatedByUserId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedByUserId { get; set; }
        public DateTime DateDeleted { get; set; }
        public int DeletedByUserId { get; set; }
        public DateTime DateHidden { get; set; }
        public int HiddenByUserId { get; set; }
        public int Status { get; set; }
        public ICollection<AdminOrderHistoriesResponse> OrderHistories { get; set; }
        public ICollection<BaseOrderDetailItemResponse> OrderDetails { get; set; }
    }

    public class AdminOrderDetailItemResponse : BaseOrderDetailItemResponse
    {
    }

    public class AdminOrderHistoriesResponse : BaseOrderHistoriesResponse
    {
        public string Note { get; set; }
    }


    #endregion

}