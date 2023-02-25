using System;
using System.Collections.Generic;
using API.Entities;

namespace API.DTOs.Response.OptionResponse
{
    #region customer
    public class CustomerOptionResponse
    {
        public CustomerOptionColorResponse Color { get; set; }
        public List<CustomerOptionSizeResponse> Sizes { get; set; }
    }

    public class CustomerOptionColorResponse
    {
        public CustomerOptionColorResponse(string colorName, string colorCode)
        {
            ColorName = colorName;
            ColorCode = colorCode;
        }

        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }

    public class CustomerOptionSizeResponse
    {
        public string SizeName { get; set; }
        public int OptionId { get; set; }
        public double Price { get; set; }
    }
    #endregion

    #region admin

    public class AdminOptionResponse
    {
        public int Id { get; set; }   
        public string ColorName { get; set; }
        public string ColorCode { get; set; }    
        public string SizeName { get; set; }
        public AdminOptionProductResponse Product { get; set; }
        public double AdditionalPrice { get; set; }
        public Status Status { get; set; }
    }

     public class AdminOptionDetailResponse : AdminOptionResponse
    {
        public DateTime DateCreated { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedByUserId { get; set; }
        public DateTime DateDeleted { get; set; }
        public int DeletedByUserId { get; set; }
        public DateTime DateHidden { get; set; }
        public int HiddenByUserId { get; set; }
    }

    public class AdminOptionProductResponse
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
    }

    #endregion

}