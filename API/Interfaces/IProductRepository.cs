using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.Entities.ProductEntities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IProductRepository
    {
        //Customer
        Task<PagedList<CustomerProductDto>> GetProductsAsCustomerAsync(ProductParams productParams);
        Task<CustomerProductDto> GetProductAsCustomerByIdAsync(int id);

        //Admin
        Task<Product> GetProductByIdAsync(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
    }
}