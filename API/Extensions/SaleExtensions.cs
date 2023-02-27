using System;
using System.Text.Json;
using API.Entities.ProductModel;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class SaleExtensions
    {
        public static bool IsProductOnSale(Product product)
        {
            if (product.SaleType == ProductSaleOffType.None)
                return false;
            var now = DateTime.UtcNow;
            if (product.SaleOffFrom > now || product.SaleOffTo < now)
                return false;
            return true;
        }

        public static double CalculatePriceAfterSaleOff(Option option)
        {
            var optionPrice = option.Product.Price + option.AdditionalPrice;
            if (option.Product.SaleType == ProductSaleOffType.SaleOffValue)
            {
                return optionPrice - option.Product.SaleOffValue;
            }
            else
            {
                return optionPrice - ((optionPrice * option.Product.SaleOffPercent) / 100);
            }
        }
    }
}