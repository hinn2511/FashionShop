using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Entities.User;
using API.Entities.UserModel;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        #region user cart

        public async Task<IEnumerable<Cart>> GetCartItemsByUserId(int userId)
        {
            return await _context.Carts.Include(x => x.Option).ThenInclude(x => x.Product).Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Cart>> GetCartItemsByUserIdAndOptions(int userId, IEnumerable<int> optionIds)
        {
            return await _context.Carts.Where(x => x.UserId == userId && optionIds.Contains(x.OptionId)).ToListAsync();
        }

        public async Task<Cart> GetCartItemById(int id)
        {
            return await _context.Carts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Cart> GetCartItemByUserIdAndOptId(int userId, int optId)
        {
            return await _context.Carts.FirstOrDefaultAsync(x => x.UserId == userId && x.OptionId == optId);
        }
    

        public void AddItemToCart(Cart cart)
        {
            _context.Carts.Add(cart);
        }

        public void RemoveItemFromCart(Cart cart)
        {
            _context.Remove(cart);
        }

        public void BulkRemove(IEnumerable<Cart> cartItems)
        {
            _context.Carts.RemoveRange(cartItems);
        }

        public void UpdateCartItem(Cart cart)
        {
            _context.Entry(cart).State = EntityState.Modified;
        }

        #endregion

        #region user like
        public void AddToFavorite(UserLike userLike)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveFromFavorite(UserLike userLike)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<UserLike>> GetAllUserLikes(int userId)
        {
            return await _context.UserLikes.Include(x => x.Product).Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<UserLike> GetUserLike(int userId, int productId)
        {
            return await _context.UserLikes.Include(x => x.Product).FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
        }

        #endregion

        #region user
        public async Task<CustomerDto> GetUserAsync(string username)
        {
            return await _context.Users
             .Where(u => u.UserName == username)
             .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
             .FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        #endregion user


    }
}