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
            
            if(productParams.Gender != null)
                query = query.Where(p => p.Category.Gender == productParams.Gender);

            if(productParams.Category != null)
                query = query.Where(p => p.Category.CategoryName == productParams.Category);

            if(!string.IsNullOrEmpty(productParams.Query))
            {   
                var words = productParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach(var word in words)
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

            query = query.Include(x => x.ProductPhotos.Where(x => x.Status == Status.Active));            

            return await PagedList<Product>.CreateAsync(query, productParams.PageNumber, productParams.PageSize);
        }

        public async Task<PagedList<Product>> GetProductsAsync(AdministratorProductParams productParams)
        {
            var query = _context.Products.AsQueryable();

            query = query.Where(x => productParams.ProductStatus.Contains(x.Status));
            
            if(productParams.Gender != null)
                query = query.Where(p => p.Category.Gender == productParams.Gender);

            if(productParams.Category != null)
                query = query.Where(p => p.Category.CategoryName == productParams.Category);

            if(!string.IsNullOrEmpty(productParams.Query))
            {   
                var words = productParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach(var word in words)
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
                        .Include(x => x.Brand)
                        .Include(x => x.SubCategory)
                        .Include(x => x.ProductPhotos)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == productId);

            return product;
        }
    }
    public class ColorRepository : GenericRepository<Color>, IColorRepository
    {
        public ColorRepository(DataContext context, DbSet<Color> set) : base(context, set)
        {
        }
    }

    public class ProductOptionRepository : GenericRepository<Option>, IProductOptionRepository
    {
        private readonly DataContext _context;

        public ProductOptionRepository(DataContext context, DbSet<Option> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<IEnumerable<Option>> GetProductOptionsAsync(int productId)
        {
            return await _context.Options
                        .Include(x => x.Color)
                        .Include(x => x.Size)
                        .Where(x => x.ProductId == productId)
                        .ToListAsync();
        }
    }

    public class SizeRepository : GenericRepository<Entities.ProductModel.Size>, ISizeRepository
    {
        public SizeRepository(DataContext context, DbSet<Entities.ProductModel.Size> set) : base(context, set)
        {
        }
    }
    
    public class StockRepository : GenericRepository<Stock>, IStockRepository
    {
        public StockRepository(DataContext context, DbSet<Stock> set) : base(context, set)
        {
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