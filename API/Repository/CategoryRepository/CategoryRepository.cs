using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.Entities;
using API.Entities.ProductModel;
using API.Extensions;
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
        private readonly DataContext _context;

        public CategoryRepository(DataContext context, DbSet<Category> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesByProductFilterAsync(CustomerProductParams customerProductParams)
        {
            var query = _context.Products.AsQueryable();

            query = query.Where(x => x.Status != Status.Hidden && x.Status != Status.Deleted);
            
            if(customerProductParams.Gender != null)
                query = query.Where(p => p.Category.Gender == customerProductParams.Gender);

            if(customerProductParams.Category != null)
                query = query.Where(p => p.Category.CategoryName == customerProductParams.Category);

            if(!string.IsNullOrEmpty(customerProductParams.Query))
            {   
                var words = customerProductParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach(var word in words)
                {
                    query = query.Where(x => x.ProductName.ToUpper().Contains(word));
                }
            }

            var categoryIds = await query.Select(x => x.Category.Id).Distinct().ToListAsync();

            return await _context.Categories.Where(x => categoryIds.Contains(x.Id)).Include(x => x.SubCategories).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithSubCategoriesAsync()
        {
            return await _context.Categories.Include(x => x.SubCategories).AsNoTracking().ToListAsync();
        }
    }

    public class SubCategoryRepository : GenericRepository<SubCategory>, ISubCategoryRepository
    {
        public SubCategoryRepository(DataContext context, DbSet<SubCategory> set) : base(context, set)
        {
        }
    }
}