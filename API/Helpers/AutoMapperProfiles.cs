using System.Linq;
using System.Net;
using API.DTOs;
using API.DTOs.Customer;
using API.DTOs.Order;
using API.DTOs.Product;
using API.DTOs.Request.ConfigurationRequest;
using API.DTOs.Request.ProductRequest;
using API.DTOs.Response;
using API.DTOs.Response.OptionResponse;
using API.Entities;
using API.Entities.OrderModel;
using API.Entities.Other;
using API.Entities.ProductModel;
using API.Entities.User;
using API.Entities.WebPageModel;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // authentication
            CreateMap<RegisterDto, AppUser>();

            // Admin product

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


            // Customer

            CreateMap<Product, CustomerProductsResponse>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.ProductPhotos.FirstOrDefault(pp => pp.IsMain).Url));

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
                          src => src.SubCategory.CategoryName));

            CreateMap<ProductPhoto, CustomerProductPhotoResponse>();

            CreateMap<Color, OptionColorResponse>();

            CreateMap<Size, OptionSizeResponse>();
            
            

            CreateMap<Product, ProductDto>()
               .ForMember(dest => dest.Category, opt => opt.MapFrom(
                          src => src.Category.CategoryName));

            

            

            CreateMap<ProductPhoto, CustomerProductPhotoDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.Url));

            CreateMap<Option, CustomerProductStockDto>();

            CreateMap<Category, CustomerCategoryDto>();

            CreateMap<Category, CategoryDto>();

            CreateMap<AddCategoryDto, Category>();

            CreateMap<UpdateCategoryDto, Category>();

            CreateMap<ProductPhoto, ProductPhotoDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.Url))
                .ForMember(dest => dest.PhotoId, opt => opt.MapFrom(
                          src => src.Id));

            CreateMap<OrderItemRequest, OrderDetail>();

            CreateMap<HomePageRequest, HomePage>();

            CreateMap<CarouselRequest, Carousel>();

            CreateMap<FeatureCategoryRequest, FeatureCategory>();

            CreateMap<FeatureProductRequest, FeatureProduct>();

        }
    }
}