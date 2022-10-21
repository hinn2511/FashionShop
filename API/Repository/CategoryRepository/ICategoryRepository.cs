using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.Entities.ProductModel;
using API.Repository.GenericRepository;

namespace API.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesWithSubCategoriesAsync();
        Task<IEnumerable<Category>> GetCategoriesByProductFilterAsync(CustomerProductParams customerProductParams);
    }

    public interface ISubCategoryRepository : IGenericRepository<SubCategory>
    {

    }
}