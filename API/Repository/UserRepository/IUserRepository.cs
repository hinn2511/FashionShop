using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Entities.User;
using API.Entities.UserModel;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        #region user
        void Update(AppUser user);
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<CustomerDto> GetUserAsync(string username);

        #endregion

        #region user like
        void AddToFavorite(UserLike userLike);
        void RemoveFromFavorite(UserLike userLike);
        Task<IEnumerable<UserLike>> GetAllUserLikes(int userId);
        Task<UserLike> GetUserLike(int userId, int productId);

        #endregion

        #region user cart
        Task<IEnumerable<Cart>> GetCartItemsByUserId(int userId);
        Task<IEnumerable<Cart>> GetCartItemsByUserIdAndOptions(int userId, IEnumerable<int> optionIds);
        Task<Cart> GetCartItemById(int id);
        Task<Cart> GetCartItemByUserIdAndOptId(int userId, int optId);

        void AddItemToCart(Cart cart);
        void RemoveItemFromCart(Cart cart);
        void BulkRemove(IEnumerable<Cart> cartItems);
        void UpdateCartItem(Cart cart);

        #endregion
    }
}