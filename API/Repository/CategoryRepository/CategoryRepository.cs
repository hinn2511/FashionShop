using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.Entities;
using API.Entities.ProductModel;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Repository.GenericRepository;
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

            if (customerProductParams.Gender != null)
                query = query.Where(p => p.Category.Gender == customerProductParams.Gender);

            if (customerProductParams.CategoryId > 0)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == customerProductParams.CategoryId);
                if (category != null)
                {
                    if (category.ParentId == 0)
                        query = query.Where(x => x.Category.Id == customerProductParams.CategoryId || x.Category.ParentId == customerProductParams.CategoryId);
                    else
                        query = query.Where(x => x.Category.Id == customerProductParams.CategoryId);

                }
            }
            
            if (!string.IsNullOrEmpty(customerProductParams.Query))
            {
                var words = customerProductParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach (var word in words)
                {
                    query = query.Where(x => x.ProductName.ToUpper().Contains(word));
                }
            }

            if(customerProductParams.IsOnSale)
            {
                var now = DateTime.UtcNow;
                query = query.Where(p => p.SaleType != ProductSaleOffType.None && p.SaleOffFrom < now && p.SaleOffTo > now);
            }

            var categoryIds = await query.Select(x => x.Category.Id).Distinct().ToListAsync();

            return await _context.Categories.Where(x => categoryIds.Contains(x.Id)).Include(x => x.SubCategories).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithSubCategoriesAsync()
        {
            return await _context.Categories.Include(x => x.SubCategories).AsNoTracking().ToListAsync();
        }

        public async Task<PagedList<Category>> GetCategoriesAsync(AdminCategoryParams adminCategoryParams)
        {
            var query = _context.Categories.AsQueryable();

            query = query.Where(x => adminCategoryParams.CategoryStatus.Contains(x.Status));

            if(adminCategoryParams.ParentId > 0)
            {
                query = query.Where(x => x.ParentId == adminCategoryParams.ParentId);
            }
            else
            {
                query = query.Where(x => adminCategoryParams.Genders.Contains(x.Gender));
            }

            if (!string.IsNullOrEmpty(adminCategoryParams.Query))
            {
                var words = adminCategoryParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach (var word in words)
                {
                    query = query.Where(x => x.CategoryName.ToUpper().Contains(word));
                }
            }


            if (adminCategoryParams.OrderBy == OrderBy.Ascending)
            {
                query = adminCategoryParams.Field switch
                {
                    "date" => query.OrderBy(p => p.DateCreated),
                    "categoryName" => query.OrderBy(p => p.CategoryName),
                    "status" => query.OrderBy(p => p.Status),
                    "gender" => query.OrderBy(p => p.Gender),
                    "promoted" => query.OrderBy(p => p.IsPromoted),
                    "parentCategory" => query.OrderBy(p => p.Parent.CategoryName),
                    _ => query.OrderBy(p => p.Id)
                };
            }
            else
            {
                query = adminCategoryParams.Field switch
                {
                    "date" => query.OrderByDescending(p => p.DateCreated),
                    "categoryName" => query.OrderByDescending(p => p.CategoryName),
                    "status" => query.OrderByDescending(p => p.Status),
                    "gender" => query.OrderByDescending(p => p.Gender),
                    "promoted" => query.OrderByDescending(p => p.IsPromoted),
                    "parentCategory" => query.OrderByDescending(p => p.Parent.CategoryName),
                    _ => query.OrderByDescending(p => p.Id)
                };
            }

            query = query.Include(x => x.Parent);

            return await PagedList<Category>.CreateAsync(query.AsNoTracking(), adminCategoryParams.PageNumber, adminCategoryParams.PageSize);
        }
    }
}