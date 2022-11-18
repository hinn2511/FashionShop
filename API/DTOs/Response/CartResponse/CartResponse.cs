using System.Collections.Generic;
using API.Entities;

namespace API.DTOs.Response.CartResponse
{
    public class CartResponse
    {
        public List<CartItemResponse> CartItems { get; set; }
        public double TotalPrice { get; set; }
        public int TotalItem { get; set; }
    }

    public class CartItemResponse
    {
        public int Id { get; set; }
        public int OptionId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public string  SizeName { get; set; }
        public double AdditionalPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalItemPrice { get; set; }
        public Status Status { get; set; }
    }

    public class CartResponses
    {
        public CartItemOption Option { get; set; }
        public int OptionId { get; set; }
        public int Quantity { get; set; }
        public int Id { get; set; }
        public Status Status { get; set; }
        public double Total { get; set; }
    }


    public class CartItemColor
    {
        public int Id { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }

    public class CartItemOption
    {
        public CartItemProduct Product { get; set; }
        public CartItemColor Color { get; set; }
        public CartItemSize Size { get; set; }
        public int ProductId { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public int AdditionalPrice { get; set; }
        public int Id { get; set; }
    }

    public class CartItemProduct
    {
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public int Price { get; set; }
        public int Id { get; set; }
    }

    
    public class CartItemSize
    {
        public int Id { get; set; }
        public string SizeName { get; set; }
    }





}