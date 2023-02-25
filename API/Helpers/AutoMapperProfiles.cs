using System.Linq;
using API.DTOs.Order;
using API.DTOs.Request.ConfigurationRequest;
using API.DTOs.Request.ProductRequest;
using API.DTOs.Request.AuthenticationRequest;
using API.DTOs.Request.CartRequest;
using API.DTOs.Request.CategoryRequest;
using API.DTOs.Response.AdminResponse;
using API.DTOs.Response.CartResponse;
using API.DTOs.Response.CategoryResponse;
using API.DTOs.Response.ConfigurationResponse;
using API.DTOs.Response.OptionResponse;
using API.DTOs.Response.OrderResponse;
using API.DTOs.Response.ProductResponse;
using API.Entities.OrderModel;
using API.Entities.Other;
using API.Entities.ProductModel;
using API.Entities.User;
using API.Entities.WebPageModel;
using AutoMapper;
using API.DTOs.Request.ContentRequest;
using API.DTOs.Response.ContentResponse;
using API.DTOs.ProductOptionRequest;
using API.DTOs.Response.ColorResponse;
using API.DTOs.Response.AccountResponse;
using System;
using API.DTOs.Response.ArticleResponse;
using API.DTOs.Request.ArticleRequest;
using static API.Extensions.TreeExtension;
using API.Entities.UserModel;
using API.DTOs.Response.ReviewResponse;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            #region manager dto

            CreateMap<Product, AdminProductsResponse>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.ProductPhotos.FirstOrDefault(pp => pp.IsMain).Url));

            CreateMap<Product, AdminProductDetailResponse>()
                .ForMember(dest => dest.CategoryGender, opt => opt.MapFrom(
                          src => src.Category.Gender))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.ProductPhotos.FirstOrDefault(pp => pp.IsMain).Url))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(
                          src => src.Brand.Name))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(
                          src => src.Category.Id))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(
                          src => src.Category.CategoryName));

            CreateMap<ProductPhoto, AdminProductPhotoResponse>();

            CreateMap<CreateProductRequest, Product>();

            CreateMap<UpdateProductRequest, Product>();

            CreateMap<Category, AdminCategoryResponse>()
                .ForMember(dest => dest.GenderName, opt => opt.MapFrom(
                          src => src.Gender.ToString()))
                .ForMember(dest => dest.ParentCategory, opt => opt.MapFrom(
                          src => src.Parent.CategoryName));

            CreateMap<Category, AdminCategoryDetailResponse>();

            CreateMap<Category, CategoryGender>();

            CreateMap<Category, CustomerSingleCategoryResponse>();

            CreateMap<ITree<Category>, AdminCatalogueCategoryResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(
                          src => src.Data.Id))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(
                          src => src.Data.CategoryName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(
                          src => src.Data.Gender))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(
                          src => src.Data.Slug))
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(
                          src => src.Children.Flatten(node => node.Children).ToList()));

            CreateMap<CreateCategoryRequest, Category>();

            CreateMap<UpdateCategoryRequest, Category>();

            CreateMap<Option, AdminOptionResponse>();

            // CreateMap<Color, AdminOptionColorResponse>();

            // CreateMap<Size, AdminOptionSizeResponse>();

            CreateMap<AppUser, UserRoleResponse>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(
                          src => string.Join(", ", src.UserRoles.Select(x => x.Role.Name).ToArray())));

            CreateMap<HomePageRequest, HomePage>();

            CreateMap<Carousel, AdminCarouselResponse>();

            CreateMap<Carousel, AdminCarouselDetailResponse>();

            CreateMap<AddCarouselRequest, Carousel>();

            CreateMap<FeatureCategoryRequest, FeatureCategory>();

            CreateMap<FeatureProductRequest, FeatureProduct>();

            CreateMap<Option, AdminOptionDetailResponse>();

            CreateMap<CreateProductOptionRequest, Option>();

            CreateMap<UpdateProductOptionRequest, Option>();

            CreateMap<Product, AdminOptionProductResponse>();

            // CreateMap<Color, AdminColorResponse>();

            // CreateMap<Color, AdminColorDetailResponse>();

            // CreateMap<CreateColorRequest, Color>();

            // CreateMap<UpdateColorRequest, Color>();

            // CreateMap<Size, AdminSizeResponse>();

            // CreateMap<Size, AdminSizeDetailResponse>();

            // CreateMap<CreateSizeRequest, Size>();

            // CreateMap<UpdateSizeRequest, Size>();

            CreateMap<Article, AdminArticleResponse>()
                .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(
                            src => src.DateCreated))
                .ForMember(dest => dest.PublishedBy, opt => opt.MapFrom(
                            src => $"{src.User.FirstName} {src.User.FirstName}"));

            CreateMap<Article, AdminArticleDetailResponse>()
                .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(
                            src => src.DateCreated))
                .ForMember(dest => dest.PublishedBy, opt => opt.MapFrom(
                            src => $"{src.User.FirstName} {src.User.FirstName}"));

            CreateMap<CreateArticleRequest, Article>();

            CreateMap<UpdateArticleRequest, Article>();
            #endregion

            #region customer dto

            CreateMap<RegisterRequest, AppUser>();

            CreateMap<AppUser, AccountResponse>();

            CreateMap<Product, CustomerProductsResponse>()
                // .ForMember(dest => dest.Options, opt => opt.MapFrom(
                //           src => src.Options))
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(
                          src => src.DateCreated.AddDays(7) > DateTime.UtcNow))
                .ForMember(dest => dest.IsOnSale, opt => opt.MapFrom(
                          src => src.SaleType != ProductSaleOffType.None 
                                &&  src.SaleOffFrom < DateTime.UtcNow && src.SaleOffTo > DateTime.UtcNow))
                .ForMember(dest => dest.SaleOffPercent, opt => opt.MapFrom(
                          src => src.SaleOffPercent))
                .ForMember(dest => dest.SaleOffValue, opt => opt.MapFrom(
                          src => src.SaleOffValue))
                // .ForMember(dest => dest.SaleType, opt => opt.Condition(
                //           src => src.SaleOffFrom < DateTime.UtcNow && src.SaleOffTo > DateTime.UtcNow))
                // .ForMember(dest => dest.SaleOffPercent, opt => opt.Condition(
                //           src => src.SaleOffFrom < DateTime.UtcNow && src.SaleOffTo > DateTime.UtcNow))
                // .ForMember(dest => dest.SaleOffValue, opt => opt.Condition(
                //           src => src.SaleOffFrom < DateTime.UtcNow && src.SaleOffTo > DateTime.UtcNow))
                //  .ForMember(dest => dest.SaleOffFrom, opt => opt.Condition(
                //           src => src.SaleOffFrom < DateTime.UtcNow && src.SaleOffTo > DateTime.UtcNow))
                // .ForMember(dest => dest.SaleOffTo, opt => opt.Condition(
                //           src => src.SaleOffFrom < DateTime.UtcNow && src.SaleOffTo > DateTime.UtcNow))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.ProductPhotos.FirstOrDefault(pp => pp.IsMain).Url))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(
                          src => (Gender)src.Category.Gender))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(
                          src => src.Category.CategoryName));

            CreateMap<Option, CustomerProductColorsResponse>();

            CreateMap<Product, CustomerProductDetailResponse>()
                .ForMember(dest => dest.IsNew, opt => opt.MapFrom(
                          src => src.DateCreated.AddDays(7) > DateTime.UtcNow))
                .ForMember(dest => dest.IsOnSale, opt => opt.MapFrom(
                          src => src.SaleType != ProductSaleOffType.None 
                                &&  src.SaleOffFrom < DateTime.UtcNow && src.SaleOffTo > DateTime.UtcNow))
                .ForMember(dest => dest.SaleOffPercent, opt => opt.MapFrom(
                          src => src.SaleOffPercent))
                .ForMember(dest => dest.SaleOffValue, opt => opt.MapFrom(
                          src => src.SaleOffValue))
                // .ForMember(dest => dest.SaleType, opt => opt.Condition(
                //           src => src.SaleOffFrom < DateTime.UtcNow && src.SaleOffTo > DateTime.UtcNow))
                // .ForMember(dest => dest.SaleOffPercent, opt => opt.Condition(
                //           src => src.SaleOffFrom < DateTime.UtcNow && src.SaleOffTo > DateTime.UtcNow))
                // .ForMember(dest => dest.SaleOffValue, opt => opt.Condition(
                //           src => src.SaleOffFrom < DateTime.UtcNow && src.SaleOffTo > DateTime.UtcNow))
                //  .ForMember(dest => dest.SaleOffFrom, opt => opt.Condition(
                //           src => src.SaleOffFrom < DateTime.UtcNow && src.SaleOffTo > DateTime.UtcNow))
                // .ForMember(dest => dest.SaleOffTo, opt => opt.Condition(
                //           src => src.SaleOffFrom < DateTime.UtcNow && src.SaleOffTo > DateTime.UtcNow))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.ProductPhotos.FirstOrDefault(pp => pp.IsMain).Url))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(
                          src => src.Brand.Name))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(
                          src => src.Category.CategoryName))
                .ForMember(dest => dest.CategorySlug, opt => opt.MapFrom(
                          src => src.Category.Slug))
                .ForMember(dest => dest.ParentCategory, opt => opt.MapFrom(
                          src => src.Category.Parent.CategoryName))
                .ForMember(dest => dest.ParentCategorySlug, opt => opt.MapFrom(
                          src => src.Category.Parent.Slug))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(
                          src => (Gender)src.Category.Gender));

            CreateMap<ProductPhoto, CustomerProductPhotoResponse>();

            // CreateMap<Color, CustomerOptionColorResponse>();

            // CreateMap<Size, CustomerOptionSizeResponse>();

            CreateMap<Category, CustomerCategoryResponse>();

            CreateMap<ITree<Category>, CustomerCategoryResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(
                          src => src.Data.CategoryName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(
                          src => src.Data.Gender))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(
                          src => src.Data.Slug))
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(
                          src => src.Children.Flatten(node => node.Children).ToList()));

            // CreateMap<Color, CartItemColor>();

            CreateMap<Option, ColorFilterResponse>();

            // CreateMap<Size, CartItemSize>();

            CreateMap<Product, CartItemProduct>();

            CreateMap<Option, CartItemOption>();

            CreateMap<Cart, CartItemResponse>()
                .ForMember(dest => dest.IsOnSale, opt => opt.MapFrom(
                          src => src.Option.Product.SaleType != ProductSaleOffType.None 
                                &&  src.Option.Product.SaleOffFrom < DateTime.UtcNow && src.Option.Product.SaleOffTo > DateTime.UtcNow))
                .ForMember(dest => dest.SaleOffPercent, opt => opt.MapFrom(
                          src => src.Option.Product.SaleOffPercent))
                .ForMember(dest => dest.SaleOffValue, opt => opt.MapFrom(
                          src => src.Option.Product.SaleOffValue))
                .ForMember(dest => dest.SaleType, opt => opt.MapFrom(
                          src => src.Option.Product.SaleType))
                // .ForMember(dest => dest.SaleType, opt => opt.Condition(
                //           src => src.Option.Product.SaleOffFrom < DateTime.UtcNow && src.Option.Product.SaleOffTo > DateTime.UtcNow))
                // .ForMember(dest => dest.SaleOffPercent, opt => opt.Condition(
                //           src => src.Option.Product.SaleOffFrom < DateTime.UtcNow && src.Option.Product.SaleOffTo > DateTime.UtcNow))
                // .ForMember(dest => dest.SaleOffValue, opt => opt.Condition(
                //           src => src.Option.Product.SaleOffFrom < DateTime.UtcNow && src.Option.Product.SaleOffTo > DateTime.UtcNow))
                .ForMember(dest => dest.OptionId, opt => opt.MapFrom(
                        src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(
                        src => src.Option.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(
                        src => src.Option.Product.ProductName))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(
                        src => src.Option.Product.Slug))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(
                          src => src.Option.Product.ProductPhotos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(
                        src => src.Option.Product.Price))
                          .ForMember(dest => dest.ColorName, opt => opt.MapFrom(
                          src => src.Option.ColorName))
                          .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(
                          src => src.Option.ColorCode))
                          .ForMember(dest => dest.SizeName, opt => opt.MapFrom(
                          src => src.Option.SizeName))
                          .ForMember(dest => dest.AdditionalPrice, opt => opt.MapFrom(
                          src => src.Option.AdditionalPrice))
                          .ForMember(dest => dest.TotalItemPrice, opt => opt.MapFrom(
                          src => (src.Option.CalculatePriceAfterSaleOff()) * src.Quantity));

            CreateMap<Option, CartItemOption>();

            CreateMap<CreateCartRequest, Cart>();

            CreateMap<UpdateCartRequest, Cart>();

            CreateMap<OrderRequest, Order>();

            CreateMap<OrderItemRequest, OrderDetail>();

            CreateMap<Order, CustomerOrderSummaryResponse>()
                .ForMember(dest => dest.TotalItem, opt => opt.MapFrom(
                          src => src.CalculateOrderTotalItem()))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(
                          src => src.CalculateOrderTotalPrice()));

            CreateMap<Order, CustomerOrderResponse>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(
                          src => src.OrderDetails.OrderBy(x => x.Option.Product.ProductName)))
                .ForMember(dest => dest.OrderHistories, opt => opt.MapFrom(
                          src => src.OrderHistories.OrderByDescending(x => x.DateCreated)))
                .ForMember(dest => dest.TotalItem, opt => opt.MapFrom(
                          src => src.CalculateOrderTotalItem()))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(
                          src => src.CalculateOrderTotalPrice()))
                .ForMember(dest => dest.PaymentMethodString, opt => opt.MapFrom(
                          src => src.PaymentMethod.ConvertToString()))
                .ForMember(dest => dest.CurrentStatusString, opt => opt.MapFrom(
                          src => src.CurrentStatus.ConvertToString()))
                .ForMember(dest => dest.IsFinished, opt => opt.MapFrom(
                          src => src.IsOrderFinished()));

            CreateMap<OrderHistory, CustomerOrderHistoriesResponse>()
                .ForMember(dest => dest.Note, opt => opt.MapFrom(
                          src => src.GenerateOrderNoteForCustomer()));

            CreateMap<OrderDetail, CustomerOrderItemResponse>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(
                          src => src.Option.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(
                          src => src.Option.Product.ProductName))
                .ForMember(dest => dest.OptionId, opt => opt.MapFrom(
                          src => src.Option.Id))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.Option.Product.ProductPhotos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(
                          src => src.Option.Product.Price + src.Option.AdditionalPrice))
                .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(
                          src => src.Option.ColorCode))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(
                          src => src.Option.ColorName))
                .ForMember(dest => dest.SizeName, opt => opt.MapFrom(
                          src => src.Option.SizeName))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(
                          src => (src.Option.AdditionalPrice + src.Option.Product.Price) * src.Quantity));

            CreateMap<Tuple<OrderStatus, int>, AdminOrderCountResponse>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(
                        src => src.Item1))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(
                        src => src.Item2));

            CreateMap<Order, AdminOrderSummaryResponse>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(
                          src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(
                          src => src.User.LastName))
                .ForMember(dest => dest.ShippingMethod, opt => opt.MapFrom(
                          src => src.ShippingMethod.Split().First()))
                .ForMember(dest => dest.PaymentMethodString, opt => opt.MapFrom(
                          src => src.PaymentMethod.ConvertToString()))
                .ForMember(dest => dest.TotalItem, opt => opt.MapFrom(
                          src => src.CalculateOrderTotalItem()))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(
                          src => src.CalculateOrderTotalPrice()));

            CreateMap<Order, AdminOrderResponse>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(
                          src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(
                          src => src.User.LastName))
                .ForMember(dest => dest.ShippingMethod, opt => opt.MapFrom(
                          src => src.ShippingMethod))
                // .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(
                //           src => src.OrderDetails.OrderBy(x => x.Option.Product.ProductName)))
                .ForMember(dest => dest.TotalItem, opt => opt.MapFrom(
                          src => src.CalculateOrderTotalItem()))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(
                          src => src.CalculateOrderTotalPrice()))
                .ForMember(dest => dest.PaymentMethodString, opt => opt.MapFrom(
                          src => src.PaymentMethod.ConvertToString()))
                .ForMember(dest => dest.CurrentStatusString, opt => opt.MapFrom(
                          src => src.CurrentStatus.ToString()));

            CreateMap<OrderHistory, AdminOrderHistoriesResponse>();

            CreateMap<OrderDetail, AdminOrderItemResponse>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(
                          src => src.Option.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(
                          src => src.Option.Product.ProductName))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.Option.Product.ProductPhotos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(
                          src => src.Option.Product.Price + src.Option.AdditionalPrice))
                .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(
                          src => src.Option.ColorCode))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(
                          src => src.Option.ColorName))
                .ForMember(dest => dest.SizeName, opt => opt.MapFrom(
                          src => src.Option.SizeName))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(
                          src => (src.Option.AdditionalPrice + src.Option.Product.Price) * src.Quantity));

            CreateMap<Carousel, AdminCarouselResponse>();

            CreateMap<Carousel, CustomerCarouselResponse>();

            CreateMap<FeatureProduct, CustomerFeatureProductResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(
                          src => src.Product.ProductName))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(
                          src => src.Product.ProductPhotos.FirstOrDefault(x => x.IsMain)));

            CreateMap<FeatureCategory, CustomerFeatureCategoryResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(
                          src => src.Category.CategoryName));

            CreateMap<Article, CustomerArticleResponse>()
                .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(
                            src => src.DateCreated))
                .ForMember(dest => dest.PublishedBy, opt => opt.MapFrom(
                            src => $"{src.User.FirstName} {src.User.FirstName}"));

            CreateMap<Article, CustomerArticleDetailResponse>()
                .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(
                            src => src.DateCreated))
                .ForMember(dest => dest.PublishedBy, opt => opt.MapFrom(
                            src => $"{src.User.FirstName} {src.User.FirstName}"));

            CreateMap<CreateCustomerReviewRequest, UserReview>();

            CreateMap<EditCustomerReviewRequest, UserReview>();

            CreateMap<UserReview, CustomerReviewResponse>()
                .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(
                            src => src.Option.ColorCode))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(
                            src => src.Option.ColorName))
                .ForMember(dest => dest.SizeName, opt => opt.MapFrom(
                            src => src.Option.SizeName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(
                            src => $"{src.User.FirstName} {src.User.LastName}"));

            CreateMap<UserReview, CustomerReviewedItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(
                            src => src.Option.Product.ProductName))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                            src => src.Option.Product.ProductPhotos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(
                            src => src.Option.ColorCode))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(
                            src => src.Option.ColorName))
                .ForMember(dest => dest.SizeName, opt => opt.MapFrom(
                            src => src.Option.SizeName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(
                            src => $"{src.User.FirstName} {src.User.LastName}"));

            #endregion
        }
    }


    #region auto mapper extension
    public static class AutoMapperExtensions
    {

        public static double CalculateOrderTotalPrice(this Order order)
        {
            double orderTotalPrice = 0;

            if (order.OrderDetails.Count > 0)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var additionalPrice = orderDetail.Option.AdditionalPrice;
                    var productPrice = orderDetail.Option.Product.Price;
                    var totalItemPrice = (productPrice + additionalPrice) * orderDetail.Quantity;
                    orderTotalPrice += totalItemPrice;
                }
            }
            return orderTotalPrice + order.Tax + order.ShippingFee;

        }

        public static double CalculateOrderTotalItem(this Order order)
        {
            return order.OrderDetails.Sum(x => x.Quantity);
        }

        public static string GenerateOrderNoteForCustomer(this OrderHistory orderHistory)
        {
            switch (orderHistory.OrderStatus)
            {
                case OrderStatus.Created:
                    return "Your order has been created.";
                case OrderStatus.Checking:
                    return "Your order is being checked.";
                case OrderStatus.Paid:
                    return "Your order has been paid successfully.";
                case OrderStatus.Processing:
                    return "Your order is being prepared.";
                case OrderStatus.Shipping:
                    return "Your order is being shipped to you by logistic department.";
                case OrderStatus.Shipped:
                    return "The logistic department has delivered your order.";
                case OrderStatus.Finished:
                    return "Delivering successfully. Your order is finished.";
                case OrderStatus.CancelRequested:
                    return "You has requested to cancel order.";
                case OrderStatus.Cancelled:
                    return "You order has been cancelled.";
                default:
                    return "None.";
            }
        }



        public static string ConvertToString(this PaymentMethod paymentMethod)
        {
            switch (paymentMethod)
            {
                case PaymentMethod.CreditCard:
                    return "Credit";
                case PaymentMethod.DebitCard:
                    return "Debit";
                case PaymentMethod.CashOnDelivery:
                    return "COD";
                default:
                    return "Mobile";
            }
        }

        public static string ConvertToString(this OrderStatus orderStatus)
        {
            switch (orderStatus)
            {
                case OrderStatus.CancelRequested:
                    return "Cancel Requested";
                case OrderStatus.ReturnRequested:
                    return "Return Requested";
                default:
                    return orderStatus.ToString();
            }
        }


        public static double CalculatePriceAfterSaleOff(this Option option)
        {
            var optionPrice = option.AdditionalPrice + option.Product.Price;
            if (option.Product.SaleType != ProductSaleOffType.None && option.Product.SaleOffFrom < DateTime.UtcNow && option.Product.SaleOffTo > DateTime.UtcNow)
            {
                if (option.Product.SaleType == ProductSaleOffType.SaleOffValue)
                {
                    return optionPrice - option.Product.SaleOffValue;
                }
                else
                {
                    return optionPrice - ((optionPrice * option.Product.SaleOffPercent) / 100);
                }
            }
            return optionPrice;

        }

        public static double CalculatePriceAfterSaleOff(this Product product)
        {
            if (product.SaleType != ProductSaleOffType.None && product.SaleOffFrom < DateTime.UtcNow && product.SaleOffTo > DateTime.UtcNow)
            {
                if (product.SaleType == ProductSaleOffType.SaleOffValue)
                {
                    return product.Price - product.SaleOffValue;
                }
                else
                {
                    return product.Price - ((product.Price * product.SaleOffPercent) / 100);
                }
            }
            return product.Price;

        }

        public static bool IsOrderFinished(this Order order)
        {
            return order.CurrentStatus == OrderStatus.Finished || order.CurrentStatus == OrderStatus.Cancelled || order.CurrentStatus == OrderStatus.Returned;
        }
    }

    #endregion
}