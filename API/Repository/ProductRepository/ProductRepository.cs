using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Customer;
using API.Entities;
using API.Entities.ProductModel;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public ProductRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        

        public async Task<PagedList<CustomerProductDto>> GetProductsAsCustomerAsync(ProductParams productParams)
        {
            var query = _context.Products.AsQueryable();
            
            if(productParams.Gender != null)
                query = query.Where(p => p.Category.Gender == productParams.Gender);

            if(productParams.Category != null)
                query = query.Where(p => p.Category.CategoryName == productParams.Category);

            query = productParams.OrderBy switch
            {
                "Newest" => query.OrderByDescending(p => p.DateCreated),
                "Price (high-low)" => query.OrderByDescending(p => p.Price),
                "Price (low-high)" => query.OrderBy(p => p.Price),
                _ => query.OrderByDescending(p => p.Sold)
            };

            return await PagedList<CustomerProductDto>
                .CreateAsync(query.ProjectTo<CustomerProductDto>(_mapper.ConfigurationProvider).AsNoTracking(),
                    productParams.PageNumber, productParams.PageSize);
        }
        public async Task<CustomerProductDto> GetProductAsCustomerByIdAsync(int id)
        {
            return await _context.Products
                .Where(p => p.Id == id)
                .ProjectTo<CustomerProductDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        #region product

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.ProductPhotos)
                .FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> productIds)
        {
            return await _context.Products.Where(x => productIds.Contains(x.Id)).ToListAsync();
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
        }
        public void Update(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        #endregion

        #region product option

        public async Task<IEnumerable<Option>> GetProductOptionsByIds(IEnumerable<int> optionIds)
        {
            return await _context.Options.Where(x => optionIds.Contains(x.Id)).ToListAsync();
        }

        public async Task<Option> GetProductOptionById(int optionId)
        {
            return await _context.Options.FirstOrDefaultAsync(x => x.Id == optionId);
        }

        public void Add(Option option)
        {
            _context.Options.Add(option);
        }

        #endregion

        #region product color
        public void Add(Color color)
        {
            _context.Colors.Add(color);
        }

        #endregion

        #region product size

        public void Add(Entities.ProductModel.Size size)
        {
            _context.Sizes.Add(size);
        }

        #endregion

        #region product stock

        public async Task<Stock> GetByOptionId(int optionId)
        {
            return await _context.Stocks.FirstOrDefaultAsync(x => x.OptionId == optionId);
        }

        public void Add(Stock stock)
        {
            _context.Stocks.Add(stock);
        }

        public void Update(Stock stock)
        {
            _context.Entry(stock).State = EntityState.Modified;
        }

        #endregion 
    }
}