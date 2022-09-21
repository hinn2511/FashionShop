using System.Linq;
using API.DTOs;
using API.DTOs.Customer;
using API.DTOs.Order;
using API.DTOs.Product;
using API.DTOs.Request.ConfigurationRequest;
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
            CreateMap<RegisterDto, AppUser>();

            CreateMap<Product, CustomerProductDto>()
               .ForMember(dest => dest.Category, opt => opt.MapFrom(
                          src => src.Category.CategoryName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(
                          src => src.Category.Gender))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.ProductPhotos.FirstOrDefault(pp => pp.IsMain).Photo.Url));

            CreateMap<Product, ProductDto>()
               .ForMember(dest => dest.Category, opt => opt.MapFrom(
                          src => src.Category.CategoryName));

            CreateMap<AddProductDto, Product>();

            CreateMap<UpdateProductDto, Product>();

            CreateMap<ProductPhoto, CustomerProductPhotoDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.Photo.Url));

            CreateMap<Option, CustomerProductStockDto>();

            CreateMap<Category, CustomerCategoryDto>();

            CreateMap<Category, CategoryDto>();

            CreateMap<AddCategoryDto, Category>();

            CreateMap<UpdateCategoryDto, Category>();

            CreateMap<ProductPhoto, ProductPhotoDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(
                          src => src.Photo.Url))
                .ForMember(dest => dest.PhotoId, opt => opt.MapFrom(
                          src => src.Photo.Id));

            CreateMap<OrderItemRequest, OrderDetail>();

            CreateMap<HomePageRequest, HomePage>();

            CreateMap<CarouselRequest, Carousel>();

            CreateMap<FeatureCategoryRequest, FeatureCategory>();

            CreateMap<FeatureProductRequest, FeatureProduct>();

        }
    }
}