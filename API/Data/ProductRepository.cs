using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Customer;
using API.Entities;
using API.Entities.ProductEntities;
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

        public async Task<PagedList<CustomerProductDto>> GetProductsAsCustomerAsync(ProductParams productParams)
        {
            var query = _context.Products.AsQueryable();
            
            if(productParams.Gender != null)
                query = query.Where(p => p.Category.Gender == productParams.Gender);

            if(productParams.Category != null)
                query = query.Where(p => p.Category.CategoryName == productParams.Category);

            query = productParams.OrderBy switch
            {
                "newest" => query.OrderByDescending(p => p.CreateAt),
                "highestPrice" => query.OrderByDescending(p => p.ProductPrice),
                "lowestPrice" => query.OrderBy(p => p.ProductPrice),
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

        public async Task<Product> FindProductByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Where(p => p.Id == id)
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
    }
}