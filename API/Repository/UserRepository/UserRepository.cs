using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Params;
using API.Entities;
using API.Entities.ProductModel;
using API.Entities.User;
using API.Entities.UserModel;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Repository.GenericRepository;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using static API.Interfaces.IUserRepository;

namespace API.Data
{
    #region user like
    public class UserLikeRepository : GenericRepository<UserLike>, IUserLikeRepository
    {
        private readonly DataContext _context;

        public UserLikeRepository(DataContext context, DbSet<UserLike> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<PagedList<Product>> GetUserFavoriteProductsAsync(int userId, CustomerProductParams productParams)
        {   
            var userLikes = await _context.UserLikes.Where(x => x.UserId == userId).ToListAsync();

            var query = _context.Products.AsQueryable();

            query = query.Where(x => userLikes.Select(x => x.ProductId).Contains(x.Id));

            query = query.Where(x => x.Status != Status.Hidden && x.Status != Status.Deleted);
            
            if(productParams.Gender != null)
                query = query.Where(p => p.Category.Gender == productParams.Gender);

            if(productParams.Category != null)
                query = query.Where(p => p.Category.CategoryName == productParams.Category);

            if(!string.IsNullOrEmpty(productParams.Query))
            {   
                var words = productParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach(var word in words)
                {
                    query = query.Where(x => x.ProductName.ToUpper().Contains(word));
                }
            }

            if (productParams.OrderBy == OrderBy.Ascending) 
            {
                query = productParams.Field switch
                {
                    "DateCreated" => query.OrderBy(p => p.DateCreated),
                    "Price" => query.OrderBy(p => p.Price),
                    "Name" => query.OrderBy(p => p.ProductName),
                    _ => query.OrderBy(p => p.Sold)
                };
            }
            else
            {
                query = productParams.Field switch
                {
                    "DateCreated" => query.OrderByDescending(p => p.DateCreated),
                    "Price" => query.OrderByDescending(p => p.Price),
                    "Name" => query.OrderByDescending(p => p.ProductName),
                    _ => query.OrderByDescending(p => p.Sold)
                };
            }

            query = query.Include(x => x.ProductPhotos.Where(x => x.Status == Status.Active));            

            return await PagedList<Product>.CreateAsync(query, productParams.PageNumber, productParams.PageSize);
        }
    }

    #endregion

    #region user like
    public class UserRepository : GenericRepository<AppUser>, IUserRepository
    {
        public UserRepository(DataContext context, DbSet<AppUser> set) : base(context, set)
        {
        }
    }

    #endregion


   #region cart
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly DataContext _context;

        public CartRepository(DataContext context, DbSet<Cart> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetUserCartItems(int userId)
        {
            return await _context.Carts
                        .Where(x => x.UserId == userId)
                        .Include(x => x.Option).ThenInclude(o => o.Product)
                        .Include(x => x.Option).ThenInclude(o => o.Color)
                        .Include(x => x.Option).ThenInclude(o => o.Size)
                        .OrderByDescending(x => x.DateCreated)
                        .AsNoTracking()
                        .ToListAsync();
        }
    }

    #endregion



}