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

            // if (!string.IsNullOrEmpty(productParams.Category))
            //     query = query.Where(p => p.Category.Slug == productParams.Category);

            if (!string.IsNullOrEmpty(productParams.Size))
            {
                var productsWithSize = _context.Options.Where(o => o.Size.SizeName.ToUpper() == productParams.Size.ToUpper());
                query = query.Where(x => productsWithSize.Select(o => o.ProductId).Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(productParams.ColorCode))
            {
                var productsWithColor = _context.Options.Where(o => o.Color.ColorCode.ToUpper() == productParams.ColorCode.ToUpper());
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
                        .ThenInclude(x => x.Parent)
                        .Include(x => x.Brand)
                        .Include(x => x.ProductPhotos)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == productId);

            return product;
        }
    }
    public class ColorRepository : GenericRepository<Color>, IColorRepository
    {
        private readonly DataContext _context;
        public ColorRepository(DataContext context, DbSet<Color> set) : base(context, set)
        {
            _context = context;

        }

        public async Task<IEnumerable<Color>> GetColorsByProductFilter(CustomerProductParams productParams)
        {
            var query = _context.Products.AsQueryable();

            query = query.Where(x => x.Status != Status.Hidden && x.Status != Status.Deleted);

            query = query.Include(x => x.Category);

            if (productParams.Gender != null)
                query = query.Where(p => p.Category.Gender == productParams.Gender);

            if (productParams.MinPrice > 0)
                query = query.Where(p => p.Price >= productParams.MinPrice);

            if (productParams.MaxPrice > 0)
                query = query.Where(p => p.Price <= productParams.MaxPrice);

            if (!string.IsNullOrEmpty(productParams.Category))
                query = query.Where(p => p.Category.Slug == productParams.Category);

            if (!string.IsNullOrEmpty(productParams.Size))
            {
                var productsWithSize = _context.Options.Where(o => o.Size.SizeName.ToUpper() == productParams.Size.ToUpper());
                query = query.Where(x => productsWithSize.Select(o => o.ProductId).Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(productParams.ColorCode))
            {
                var productsWithColorCode = _context.Options.Where(o => o.Color.ColorCode == productParams.ColorCode);
                query = query.Where(x => productsWithColorCode.Select(o => o.ProductId).Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(productParams.Query))
            {
                var words = productParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach (var word in words)
                {
                    query = query.Where(x => x.ProductName.ToUpper().Contains(word));
                }
            }

            var products = await query.AsNoTracking().ToListAsync();

            var productIds = products.Select(x => x.Id);

            var options = await _context.Options.Where(o => productIds.Contains(o.ProductId) && o.Status == Status.Active).ToListAsync();

            var colorIds = options.Select(x => x.ColorId).Distinct();

            return await _context.Colors.Where(c => colorIds.Contains(c.Id)).ToListAsync();


            // if (productParams.OrderBy == OrderBy.Ascending)
            // {
            //     query = productParams.Field switch
            //     {
            //         "DateCreated" => query.OrderBy(p => p.DateCreated),
            //         "Price" => query.OrderBy(p => p.Price),
            //         "Name" => query.OrderBy(p => p.ProductName),
            //         _ => query.OrderBy(p => p.Sold)
            //     };
            // }
            // else
            // {
            //     query = productParams.Field switch
            //     {
            //         "DateCreated" => query.OrderByDescending(p => p.DateCreated),
            //         "Price" => query.OrderByDescending(p => p.Price),
            //         "Name" => query.OrderByDescending(p => p.ProductName),
            //         _ => query.OrderByDescending(p => p.Sold)
            //     };
            // }

        }
    }

    public class ProductOptionRepository : GenericRepository<Option>, IProductOptionRepository
    {
        private readonly DataContext _context;

        public ProductOptionRepository(DataContext context, DbSet<Option> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<PagedList<Option>> GetProductOptionsAsAdminAsync(AdminProductOptionParams productOptionParams)
        {
            var query = _context.Options.AsQueryable();

            query = query.Where(x => productOptionParams.ProductOptionStatus.Contains(x.Status));

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
                                                x.Color.ColorName.ToUpper().Contains(word) ||
                                                x.Color.ColorCode.ToUpper().Contains(word) ||
                                                x.Size.SizeName.ToUpper().Contains(word));
                    }
                }
            }

            if (productOptionParams.OrderBy == OrderBy.Ascending)
            {
                query = productOptionParams.Field switch
                {
                    "ColorCode" => query.OrderBy(p => p.Color.ColorCode),
                    "ColorName" => query.OrderBy(p => p.Color.ColorName),
                    "SizeName" => query.OrderBy(p => p.Size.SizeName),
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
                    "ColorCode" => query.OrderByDescending(p => p.Color.ColorCode),
                    "ColorName" => query.OrderByDescending(p => p.Color.ColorName),
                    "SizeName" => query.OrderByDescending(p => p.Size.SizeName),
                    "AdditionalPrice" => query.OrderByDescending(p => p.AdditionalPrice),
                    "ProductName" => query.OrderByDescending(p => p.Product.ProductName),
                    "ProductId" => query.OrderByDescending(p => p.Product.Id),
                    "Status" => query.OrderByDescending(p => p.Status),
                    "Id" => query.OrderByDescending(p => p.Id),
                    _ => query.OrderByDescending(p => p.DateCreated)
                };
            }

            query = query.Include(x => x.Color).Include(x => x.Size).Include(x => x.Product);

            return await PagedList<Option>.CreateAsync(query, productOptionParams.PageNumber, productOptionParams.PageSize);
        }

        public async Task<IEnumerable<Option>> GetProductOptionsAsCustomerAsync(int productId)
        {
            return await _context.Options
                        .Include(x => x.Color)
                        .Include(x => x.Size)
                        .Include(x => x.Product)
                        .Where(x => x.ProductId == productId && x.Status == Status.Active)
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