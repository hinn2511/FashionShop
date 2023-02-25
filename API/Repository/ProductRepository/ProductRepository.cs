using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Params;
using API.Entities;
using API.Entities.Other;
using API.Entities.ProductModel;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Repository.GenericRepository;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context, DbSet<Product> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<PagedList<Product>> GetProductsAsync(CustomerProductParams productParams)
        {
            var query = _context.Products.AsQueryable();

            query = query.Where(x => x.Status != Status.Hidden && x.Status != Status.Deleted);

            if (productParams.Gender != null)
                query = query.Where(p => p.Category.Gender == productParams.Gender);

            if (productParams.MinPrice > 0)
                query = query.Where(p => p.Price >= productParams.MinPrice);

            if (productParams.MaxPrice > 0)
                query = query.Where(p => p.Price <= productParams.MaxPrice);

            if(!string.IsNullOrEmpty(productParams.Category))
            {
                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Slug == productParams.Category);

                if (category.ParentId == null)
                {
                    query = query.Where(p => p.Category.ParentId == category.Id || p.Category.Id == category.Id);
                }
                else
                    query = query.Where(p => p.Category.Id == category.Id);
            }

            if (!string.IsNullOrEmpty(productParams.Size))
            {
                var productsWithSize = _context.Options.Where(o => o.SizeName.ToUpper() == productParams.Size.ToUpper());
                query = query.Where(x => productsWithSize.Select(o => o.ProductId).Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(productParams.ColorCode))
            {
                var productsWithColor = _context.Options.Where(o => o.ColorCode.ToUpper() == productParams.ColorCode.ToUpper());
                query = query.Where(x => productsWithColor.Select(o => o.ProductId).Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(productParams.Query))
            {
                var words = productParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach (var word in words)
                {
                    query = query.Where(x => x.ProductName.ToUpper().Contains(word));
                }
            }


            if (productParams.OrderBy == OrderBy.Ascending)
            {
                query = productParams.Field switch
                {
                    "DateCreated" => query.OrderBy(p => p.DateCreated),
                    "Price" => query.OrderBy(p => p.Price),
                    "Name" => query.OrderBy(p => p.ProductName),
                    _ => query.OrderBy(p => p.Sold)
                };
            }
            else
            {
                query = productParams.Field switch
                {
                    "DateCreated" => query.OrderByDescending(p => p.DateCreated),
                    "Price" => query.OrderByDescending(p => p.Price),
                    "Name" => query.OrderByDescending(p => p.ProductName),
                    _ => query.OrderByDescending(p => p.Sold)
                };
            }


            query = query.Include(x => x.ProductPhotos.Where(x => x.Status == Status.Active)).Include(x => x.Category);

            return await PagedList<Product>.CreateAsync(query, productParams.PageNumber, productParams.PageSize);
        }

        public async Task<PagedList<Product>> GetProductsAsync(AdministratorProductParams productParams)
        {
            var query = _context.Products.AsQueryable();

            query = query.Where(x => productParams.ProductStatus.Contains(x.Status));

            if (productParams.Gender != null)
                query = query.Where(p => p.Category.Gender == productParams.Gender);

            if (productParams.Category != null)
                query = query.Where(p => p.Category.CategoryName == productParams.Category);

            if (productParams.MinPrice > 0)
                query = query.Where(p => p.Price >= productParams.MinPrice);

            if (productParams.MaxPrice > 0)
                query = query.Where(p => p.Price <= productParams.MaxPrice);

            if (!string.IsNullOrEmpty(productParams.Query))
            {
                var words = productParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach (var word in words)
                {
                    query = query.Where(x => x.ProductName.ToUpper().Contains(word));
                }
            }

            if (productParams.OrderBy == OrderBy.Ascending)
            {
                query = productParams.Field switch
                {
                    "Date" => query.OrderBy(p => p.DateCreated),
                    "Status" => query.OrderBy(p => p.Status),
                    "Sold" => query.OrderBy(p => p.Sold),
                    "Price" => query.OrderBy(p => p.Price),
                    "Name" => query.OrderBy(p => p.ProductName),
                    "Promoted" => query.OrderBy(p => p.IsPromoted),
                    _ => query.OrderBy(p => p.Id)
                };
            }
            else
            {
                query = productParams.Field switch
                {
                    "Date" => query.OrderByDescending(p => p.DateCreated),
                    "Status" => query.OrderByDescending(p => p.Status),
                    "Sold" => query.OrderByDescending(p => p.Sold),
                    "Price" => query.OrderByDescending(p => p.Price),
                    "Name" => query.OrderByDescending(p => p.ProductName),
                    "Promoted" => query.OrderByDescending(p => p.IsPromoted),
                    _ => query.OrderByDescending(p => p.Id)
                };
            }

            query = query.Include(x => x.ProductPhotos);

            return await PagedList<Product>.CreateAsync(query, productParams.PageNumber, productParams.PageSize);
        }

        public async Task<Product> GetProductDetailWithPhotoAsync(int productId)
        {

            var product = await _context.Products
                        .Include(x => x.Category)
                        .ThenInclude(x => x.Parent)
                        .Include(x => x.Brand)
                        .Include(x => x.ProductPhotos)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == productId);

            return product;
        }
    }

    public class ProductOptionRepository : GenericRepository<Option>, IProductOptionRepository
    {
        private readonly DataContext _context;

        public ProductOptionRepository(DataContext context, DbSet<Option> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<IEnumerable<Option>> GetOptionsByProductParams(CustomerProductParams productParams)
        {
            var query = _context.Products.AsQueryable();

            query = query.Where(x => x.Status != Status.Hidden && x.Status != Status.Deleted);

            if (productParams.Gender != null)
                query = query.Where(p => p.Category.Gender == productParams.Gender);

            if (productParams.MinPrice > 0)
                query = query.Where(p => p.Price >= productParams.MinPrice);

            if (productParams.MaxPrice > 0)
                query = query.Where(p => p.Price <= productParams.MaxPrice);

            if(!string.IsNullOrEmpty(productParams.Category))
            {
                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Slug == productParams.Category);

                if (category.ParentId == null)
                {
                    query = query.Where(p => p.Category.ParentId == category.Id || p.Category.Id == category.Id);
                }
                else
                    query = query.Where(p => p.Category.Id == category.Id);
            }

            if (!string.IsNullOrEmpty(productParams.Size))
            {
                var productsWithSize = _context.Options.Where(o => o.SizeName.ToUpper() == productParams.Size.ToUpper());
                query = query.Where(x => productsWithSize.Select(o => o.ProductId).Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(productParams.ColorCode))
            {
                var productsWithColor = _context.Options.Where(o => o.ColorCode.ToUpper() == productParams.ColorCode.ToUpper());
                query = query.Where(x => productsWithColor.Select(o => o.ProductId).Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(productParams.Query))
            {
                var words = productParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach (var word in words)
                {
                    query = query.Where(x => x.ProductName.ToUpper().Contains(word));
                }
            }


            if (productParams.OrderBy == OrderBy.Ascending)
            {
                query = productParams.Field switch
                {
                    "DateCreated" => query.OrderBy(p => p.DateCreated),
                    "Price" => query.OrderBy(p => p.Price),
                    "Name" => query.OrderBy(p => p.ProductName),
                    _ => query.OrderBy(p => p.Sold)
                };
            }
            else
            {
                query = productParams.Field switch
                {
                    "DateCreated" => query.OrderByDescending(p => p.DateCreated),
                    "Price" => query.OrderByDescending(p => p.Price),
                    "Name" => query.OrderByDescending(p => p.ProductName),
                    _ => query.OrderByDescending(p => p.Sold)
                };
            }

            var productIds = await query.Select(x => x.Id).ToListAsync();

            return await _context.Options
                        .Where(x => productIds.Contains(x.ProductId) && x.Status == Status.Active)
                        .ToListAsync();
        }

        public async Task<PagedList<Option>> GetProductOptionsAsAdminAsync(AdminProductOptionParams productOptionParams)
        {
            var query = _context.Options.AsQueryable();

            query = query.Where(x => productOptionParams.ProductOptionStatus.Contains(x.Status));

            if (productOptionParams.ProductIds != null)
            {
                query = query.Where(x => productOptionParams.ProductIds.Contains(x.ProductId));
            }

            if (!string.IsNullOrEmpty(productOptionParams.Query))
            {
                if (productOptionParams.Query.Contains("#"))
                {
                    int id = int.TryParse(productOptionParams.Query.RemoveSpecialCharacters(), out id) ? id : 0;
                    if (id != 0)
                        query = query.Where(x => x.ProductId == id);
                }
                else
                {
                    var words = productOptionParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                    foreach (var word in words)
                    {
                        query = query.Where(x => x.Product.ProductName.ToUpper().Contains(word) ||
                                                x.ColorName.ToUpper().Contains(word) ||
                                                x.ColorCode.ToUpper().Contains(word) ||
                                                x.SizeName.ToUpper().Contains(word));
                    }
                }
            }

            if (productOptionParams.OrderBy == OrderBy.Ascending)
            {
                query = productOptionParams.Field switch
                {
                    "ColorCode" => query.OrderBy(p => p.ColorCode),
                    "ColorName" => query.OrderBy(p => p.ColorName),
                    "SizeName" => query.OrderBy(p => p.SizeName),
                    "AdditionalPrice" => query.OrderBy(p => p.AdditionalPrice),
                    "ProductName" => query.OrderBy(p => p.Product.ProductName),
                    "ProductId" => query.OrderBy(p => p.Product.Id),
                    "Status" => query.OrderBy(p => p.Status),
                    "Id" => query.OrderBy(p => p.Id),
                    _ => query.OrderBy(p => p.DateCreated)
                };
            }
            else
            {
                query = productOptionParams.Field switch
                {
                    "ColorCode" => query.OrderByDescending(p => p.ColorCode),
                    "ColorName" => query.OrderByDescending(p => p.ColorName),
                    "SizeName" => query.OrderByDescending(p => p.SizeName),
                    "AdditionalPrice" => query.OrderByDescending(p => p.AdditionalPrice),
                    "ProductName" => query.OrderByDescending(p => p.Product.ProductName),
                    "ProductId" => query.OrderByDescending(p => p.Product.Id),
                    "Status" => query.OrderByDescending(p => p.Status),
                    "Id" => query.OrderByDescending(p => p.Id),
                    _ => query.OrderByDescending(p => p.DateCreated)
                };
            }

            query = query.Include(x => x.Product);

            return await PagedList<Option>.CreateAsync(query, productOptionParams.PageNumber, productOptionParams.PageSize);
        }

        public async Task<IEnumerable<Option>> GetProductOptionsAsCustomerAsync(int productId)
        {
            return await _context.Options
                        .Include(x => x.Product)
                        .Where(x => x.ProductId == productId && x.Status == Status.Active)
                        .ToListAsync();
        }
    }

   

    public class ProductPhotoRepository : GenericRepository<ProductPhoto>, IProductPhotoRepository
    {
        private readonly DataContext _context;

        public ProductPhotoRepository(DataContext context, DbSet<ProductPhoto> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<ProductPhoto> GetLastPhotoAsync()
        {
            return await _context.ProductPhotos.OrderBy(x => x.Id).LastOrDefaultAsync();
        }
    }

}