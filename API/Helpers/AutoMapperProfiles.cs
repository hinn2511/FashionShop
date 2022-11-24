using System.Linq;
using System.Net;
using API.DTOs;
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
using API.Extensions;
using AutoMapper;
using API.DTOs.Request.ContentRequest;
using API.DTOs.Response.ContentResponse;
using API.DTOs.ProductOptionRequest;
using API.DTOs.Response.ColorResponse;
using API.DTOs.Request.ColorRequest;
using API.DTOs.Response.SizeResponse;
using API.DTOs.Request.SizeRequest;
using API.DTOs.Response.AccountResponse;
using API.DTOs.Request.AccountRequest;

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
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.ProductPhotos.FirstOrDefault(pp => pp.IsMain).Url))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(
                          src => src.Brand.Name))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(
                          src => src.Category.Id))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(
                          src => src.Category.CategoryName))
                .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(
                          src => src.SubCategory.Id))
                .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(
                          src => src.SubCategory.CategoryName));

            CreateMap<ProductPhoto, AdminProductPhotoResponse>();

            CreateMap<CreateProductRequest, Product>();

            CreateMap<UpdateProductRequest, Product>();

            CreateMap<Category, AdminCategoryResponse>();

            CreateMap<SubCategory, AdminSubCategoryResponse>();

            CreateMap<SubCategory, SubCategoryGender>();

            CreateMap<Category, AdminCategoryDetailResponse>();

            CreateMap<Category, CategoryGender>();

            CreateMap<CreateCategoryRequest, Category>();

            CreateMap<CreateCategoryRequest, SubCategory>();

            CreateMap<Option, AdminOptionResponse>();

            CreateMap<Color, AdminOptionColorResponse>();

            CreateMap<Size, AdminOptionSizeResponse>();

            CreateMap<AppUser, UserRoleResponse>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(
                          src => string.Join(", ", src.UserRoles.Select(x => x.Role.Name).ToArray())));

            CreateMap<HomePageRequest, HomePage>();

            CreateMap<Carousel, AdminCarouselResponse>();

            CreateMap<Carousel, AdminCarouselDetailResponse>();

            CreateMap<AddCarouselRequest, Carousel>();

            CreateMap<FeatureCategoryRequest, FeatureCategory>();

            CreateMap<FeatureProductRequest, FeatureProduct>();

            CreateMap<CreateProductOptionRequest, Option>();

            CreateMap<Option, AdminOptionDetailResponse>();

            CreateMap<Product, AdminOptionProductResponse>();

            CreateMap<Color, AdminColorResponse>();

            CreateMap<Color, AdminColorDetailResponse>();

            CreateMap<CreateColorRequest, Color>();

            CreateMap<UpdateColorRequest, Color>();

            CreateMap<Size, AdminSizeResponse>();

            CreateMap<Size, AdminSizeDetailResponse>();

            CreateMap<CreateSizeRequest, Size>();

            CreateMap<UpdateSizeRequest, Size>();

            #endregion

            #region customer dto

            CreateMap<RegisterRequest, AppUser>();

            CreateMap<AppUser, AccountResponse>();

            CreateMap<Product, CustomerProductsResponse>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.ProductPhotos.FirstOrDefault(pp => pp.IsMain).Url))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(
                          src => (Gender)src.Category.Gender))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(
                          src => src.Category.CategoryName))
                .ForMember(dest => dest.SubCategory, opt => opt.MapFrom(
                          src => src.SubCategory.CategoryName));

            CreateMap<Product, CustomerProductDetailResponse>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.ProductPhotos.FirstOrDefault(pp => pp.IsMain).Url))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(
                          src => src.Brand.Name))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(
                          src => src.Category.Id))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(
                          src => src.Category.CategoryName))
                .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(
                          src => src.SubCategory.Id))
                .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(
                          src => src.SubCategory.CategoryName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(
                          src => (Gender)src.Category.Gender));

            CreateMap<ProductPhoto, CustomerProductPhotoResponse>();

            CreateMap<Color, CustomerOptionColorResponse>();

            CreateMap<Size, CustomerOptionSizeResponse>();

            CreateMap<Category, CustomerCategoryResponse>();

            CreateMap<SubCategory, CustomerSubCategoryResponse>();

            CreateMap<Color, CartItemColor>();

            CreateMap<Size, CartItemSize>();

            CreateMap<Product, CartItemProduct>();

            CreateMap<Option, CartItemOption>();

            CreateMap<Cart, CartItemResponse>()
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
                          src => src.Option.Color.ColorName))
                          .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(
                          src => src.Option.Color.ColorCode))
                          .ForMember(dest => dest.SizeName, opt => opt.MapFrom(
                          src => src.Option.Size.SizeName))
                          .ForMember(dest => dest.AdditionalPrice, opt => opt.MapFrom(
                          src => src.Option.AdditionalPrice))
                          .ForMember(dest => dest.TotalItemPrice, opt => opt.MapFrom(
                          src => (src.Option.AdditionalPrice + src.Option.Product.Price) * src.Quantity));

            CreateMap<Option, CartItemOption>();

            CreateMap<CreateCartRequest, Cart>();

            CreateMap<UpdateCartRequest, Cart>();

            CreateMap<OrderRequest, Order>();

            CreateMap<OrderItemRequest, OrderDetail>();

            CreateMap<Order, CustomerOrderResponse>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => (src.OrderDetails.FirstOrDefault().Option.Product.ProductPhotos.FirstOrDefault(x => x.IsMain))))
                .ForMember(dest => dest.TotalItem, opt => opt.MapFrom(
                          src => src.CalculateOrderTotalItem()))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(
                          src => src.CalculateOrderTotalPrice()));

            CreateMap<Order, CustomerOrderDetailResponse>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => (src.OrderDetails.FirstOrDefault().Option.Product.ProductPhotos.FirstOrDefault(x => x.IsMain))))
                .ForMember(dest => dest.TotalItem, opt => opt.MapFrom(
                          src => src.CalculateOrderTotalItem()))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(
                          src => src.CalculateOrderTotalPrice()));

            CreateMap<OrderHistory, CustomerOrderHistoriesResponse>()
                .ForMember(dest => dest.Note, opt => opt.MapFrom(
                          src => src.GenerateOrderNoteForCustomer()));

            CreateMap<OrderDetail, CustomerOrderDetailItemResponse>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(
                          src => src.Option.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(
                          src => src.Option.Product.ProductName))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.Option.Product.ProductPhotos.FirstOrDefault(x => x.IsMain)))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(
                          src => src.Option.Product.Price + src.Option.AdditionalPrice))
                .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(
                          src => src.Option.Color.ColorCode))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(
                          src => src.Option.Color.ColorName))
                .ForMember(dest => dest.SizeName, opt => opt.MapFrom(
                          src => src.Option.Size.SizeName))
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

            CreateMap<Color, CustomerOptionColorResponse>();

            CreateMap<Size, CustomerOptionSizeResponse>();
            
            #endregion

            #region comment
            // CreateMap<Option, CustomerProductStockDto>();

            // CreateMap<Category, CustomerCategoryDto>();

            // CreateMap<Category, CategoryDto>();

            // CreateMap<AddCategoryDto, Category>();

            // CreateMap<UpdateCategoryDto, Category>();

            // CreateMap<ProductPhoto, ProductPhotoDto>()
            //     .ForMember(dest => dest.Url, opt => opt.MapFrom(
            //               src => src.Url))
            //     .ForMember(dest => dest.PhotoId, opt => opt.MapFrom(
            //               src => src.Id));

            // CreateMap<OrderItemRequest, OrderDetail>();



            #endregion
        }
    }


    #region automapper extension
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
            return orderTotalPrice;

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
                case OrderStatus.AwaitingPayment:
                    return "Awaiting for payment.";
                case OrderStatus.Paid:
                    return "Payment successfully.";
                case OrderStatus.Processing:
                    return "Your order is being prepared.";
                case OrderStatus.Shipping:
                    return "Your order is being shipped.";
                case OrderStatus.CancelRequested:
                    return "You has requested to cancel order.";
                case OrderStatus.Cancelled:
                    return "You order has been cancelled.";
                default:
                    return "None.";
            }

        }
    }

    #endregion
}