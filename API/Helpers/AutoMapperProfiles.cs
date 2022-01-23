using System.Linq;
using API.DTOs;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.Entities;
using API.Entities.ProductEntities;
using API.Entities.User;
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
                          src => src.Category.CategoryName));

            CreateMap<Product, ProductDto>()
               .ForMember(dest => dest.Category, opt => opt.MapFrom(
                          src => src.Category.CategoryName));

            CreateMap<AddProductDto, Product>();

            CreateMap<UpdateProductDto, Product>();

            CreateMap<ProductPhoto, CustomerProductPhotoDto>();

            CreateMap<Stock, CustomerProductStockDto>();

            CreateMap<Category, CustomerCategoryDto>()
               .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(
                          src => src.ParentCategory.CategoryName));

            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(
                       src => src.ParentCategory.CategoryName));
                       
            CreateMap<AddCategoryDto, Category>();

            CreateMap<UpdateCategoryDto, Category>();
        }
    }
}