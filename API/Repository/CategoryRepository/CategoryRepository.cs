using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.Entities.ProductModel;
using API.Interfaces;
using API.Repository.GenericRepository;
using AutoMapper;
using AutoMapper.Configuration.Conventions;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DataContext context, DbSet<Category> set) : base(context, set)
        {
        }
    }

    public class SubCategoryRepository : GenericRepository<SubCategory>, ISubCategoryRepository
    {
        public SubCategoryRepository(DataContext context, DbSet<SubCategory> set) : base(context, set)
        {
        }
    }
}