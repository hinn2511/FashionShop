using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.Entities.ProductModel;

namespace API.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CustomerCategoryDto>> GetCategoriesAsCustomerAsync(Gender gender);
        Task<Category> FindCategoryByIdAsync(int id);
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        void Add(Category category);
        void Update(Category category);
        void Delete(Category category);
    }
}