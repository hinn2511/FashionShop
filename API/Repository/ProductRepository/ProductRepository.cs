using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Customer;
using API.Entities;
using API.Entities.Other;
using API.Entities.ProductModel;
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

        public async Task<PagedList<Product>> GetProductsAsCustomerAsync(ProductParams productParams)
        {
            var query = _context.Products.AsQueryable();
            
            if(productParams.Gender != null)
                query = query.Where(p => p.Category.Gender == productParams.Gender);

            if(productParams.Category != null)
                query = query.Where(p => p.Category.CategoryName == productParams.Category);

            if (productParams.OrderBy == OrderBy.Ascending) 
            {
                query = productParams.Field switch
                {
                    "DateCreated" => query.OrderBy(p => p.DateCreated),
                    "Price" => query.OrderBy(p => p.Price),
                    "Name" => query.OrderBy(p => p.ProductName),
                    _ => query.OrderByDescending(p => p.Sold)
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

            query = query.Include(x => x.ProductPhotos);            

            return await PagedList<Product>.CreateAsync(query, productParams.PageNumber, productParams.PageSize);
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
        public ProductOptionRepository(DataContext context, DbSet<Option> set) : base(context, set)
        {
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