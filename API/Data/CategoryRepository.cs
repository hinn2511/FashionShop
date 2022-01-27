using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.Entities.ProductEntities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.Configuration.Conventions;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public CategoryRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
        }
        public void Update(Category category)
        {
            _context.Entry(category).State = EntityState.Modified;
        }

        public void Delete(Category category)
        {
            _context.Categories.Remove(category);
        }

        public async Task<IEnumerable<CustomerCategoryDto>> GetCategoriesAsCustomerAsync(string gender)
        {
            if(!gender.Equals("all"))
                return await _context.Categories
                    .Where(c => c.Gender.ToLower() == gender)
                    .ProjectTo<CustomerCategoryDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            return await _context.Categories
                    .ProjectTo<CustomerCategoryDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
        }


        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
        public async Task<Category> FindCategoryByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}