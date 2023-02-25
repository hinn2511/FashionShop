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

            if (customerProductParams.Gender != null)
                query = query.Where(p => p.Category.Gender == customerProductParams.Gender);

            if (customerProductParams.Category != null)
                query = query.Where(p => p.Category.CategoryName == customerProductParams.Category);

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
                    "PublishedDate" => query.OrderBy(p => p.DateCreated),
                    "CategoryName" => query.OrderBy(p => p.CategoryName),
                    "Status" => query.OrderBy(p => p.Status),
                    "Gender" => query.OrderBy(p => p.Gender),
                    "Promoted" => query.OrderBy(p => p.IsPromoted),
                    "ParentCategory" => query.OrderBy(p => p.Parent.CategoryName),
                    _ => query.OrderBy(p => p.Id)
                };
            }
            else
            {
                query = adminCategoryParams.Field switch
                {
                    "PublishedDate" => query.OrderByDescending(p => p.DateCreated),
                    "CategoryName" => query.OrderByDescending(p => p.CategoryName),
                    "Status" => query.OrderByDescending(p => p.Status),
                    "Gender" => query.OrderByDescending(p => p.Gender),
                    "Promoted" => query.OrderByDescending(p => p.IsPromoted),
                    "ParentCategory" => query.OrderByDescending(p => p.Parent.CategoryName),
                    _ => query.OrderByDescending(p => p.Id)
                };
            }

            query = query.Include(x => x.Parent);

            return await PagedList<Category>.CreateAsync(query.AsNoTracking(), adminCategoryParams.PageNumber, adminCategoryParams.PageSize);
        }
    }
}