using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Params;
using API.Entities;
using API.Entities.ProductModel;
using API.Entities.User;
using API.Entities.UserModel;
using API.Helpers;
using API.Repository.GenericRepository;

namespace API.Interfaces
{
    #region user
    public interface IUserRepository : IGenericRepository<AppUser>
    {
    }

    #endregion

    #region user like
    public interface IUserLikeRepository : IGenericRepository<UserLike>
    {
        Task<PagedList<Product>> GetUserFavoriteProductsAsync(int userId, CustomerProductParams productParams);
    }


    #endregion

    #region user cart
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<IEnumerable<Cart>> GetUserCartItems(int userId);
    }

    #endregion
}