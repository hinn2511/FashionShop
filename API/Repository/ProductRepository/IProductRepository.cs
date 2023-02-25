using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Params;
using API.Entities.Other;
using API.Entities.ProductModel;
using API.Helpers;
using API.Repository.GenericRepository;

namespace API.Interfaces
{
    public interface IProductPhotoRepository : IGenericRepository<ProductPhoto>
    {
    }

    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<PagedList<Product>> GetProductsAsync(CustomerProductParams productParams);

        Task<PagedList<Product>> GetProductsAsync(AdministratorProductParams productParams);

        Task<Product> GetProductDetailWithPhotoAsync(int productId);

    }

    public interface IProductOptionRepository : IGenericRepository<Option>
    {
        // Task<Option> GetProductOptionAsync(int productId);
        Task<IEnumerable<Option>> GetProductOptionsAsCustomerAsync(int productId);
        Task<PagedList<Option>> GetProductOptionsAsAdminAsync(AdminProductOptionParams productOptionParams);
        Task<IEnumerable<Option>> GetOptionsByProductParams(CustomerProductParams productParams);
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