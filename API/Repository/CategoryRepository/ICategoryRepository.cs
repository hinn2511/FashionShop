using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.Entities.ProductModel;
using API.Repository.GenericRepository;

namespace API.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
      
    }

    public interface ISubCategoryRepository : IGenericRepository<SubCategory>
    {

    }
}