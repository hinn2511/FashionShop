using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.Entities.ProductModel;
using API.Helpers;
using API.Repository.GenericRepository;

namespace API.Interfaces
{

    public interface IColorRepository : IGenericRepository<Color>
    {

    }

    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<PagedList<Product>> GetProductsAsCustomerAsync(ProductParams productParams);
    }

    public interface IProductOptionRepository : IGenericRepository<Option>
    {

    }

    public interface ISizeRepository : IGenericRepository<Size>
    {

    }

    public interface IStockRepository : IGenericRepository<Stock>
    {

    }

    // {
    //     #region customer
    //     Task<PagedList<CustomerProductDto>> GetProductsAsCustomerAsync(ProductParams productParams);
    //     Task<CustomerProductDto> GetProductAsCustomerByIdAsync(int id);

    //     #endregion

    //     #region product
    //     Task<Product> GetProductByIdAsync(int id);
    //     Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<int> productIds);
    //     void Add(Product product);
    //     void Update(Product product);
    //     void Delete(Product product);

    //     #endregion

    //     #region product option
    //     Task<IEnumerable<Option>> GetProductOptionsByIds(IEnumerable<int> optionIds);
    //     Task<IEnumerable<Option>> GetProductOptionsByProductId(int productId);
    //     Task<Option> GetProductOptionById(int optionIds);
    //     void Add(Option option);

    //     #endregion


    //     #region product size
    //     void Add(Size size);

    //     #endregion

    //     #region product stock
    //     void Add(Stock stock);
    //     Task<Stock> GetByOptionId(int optionId);
    //     void Update(Stock stock);

    //     #endregion
    // }
}