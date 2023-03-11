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
            var userLikes = await _context.UserLikes.Where(x => x.UserId == userId).OrderByDescending(x => x.DateCreated).ToListAsync();

            var query = _context.Products.AsQueryable();

            query = query.Where(x => userLikes.Select(x => x.ProductId).Contains(x.Id));

            query = query.Where(x => x.Status != Status.Hidden && x.Status != Status.Deleted);

            if (productParams.OrderBy == OrderBy.Ascending)
            {
                query = productParams.Field switch
                {
                    "dateCreated" => query.OrderBy(p => p.DateCreated),
                    "price" => query.OrderBy(p => p.Price),
                    "name" => query.OrderBy(p => p.ProductName),
                    _ => query
                };
            }
            else
            {
                query = productParams.Field switch
                {
                    "dateCreated" => query.OrderByDescending(p => p.DateCreated),
                    "price" => query.OrderByDescending(p => p.Price),
                    "name" => query.OrderByDescending(p => p.ProductName),
                    _ => query
                };
            }


            query = query.Include(x => x.Options).Include(x => x.ProductPhotos.Where(x => x.Status == Status.Active)).Include(x => x.Category);

            return await PagedList<Product>.CreateAsync(query, productParams.PageNumber, productParams.PageSize);
        }
    }

    #endregion

    #region user
    public class UserRepository : GenericRepository<AppUser>, IUserRepository
    {
        public UserRepository(DataContext context, DbSet<AppUser> set) : base(context, set)
        {
        }
    }

    public class AppRoleRepository : GenericRepository<AppRole>, IAppRoleRepository
    {
        public AppRoleRepository(DataContext context, DbSet<AppRole> set) : base(context, set)
        {
        }
    }


    public class AppRolePermissionRepository : GenericRepository<AppRolePermission>, IAppRolePermissionRepository
    {
        public AppRolePermissionRepository(DataContext context, DbSet<AppRolePermission> set) : base(context, set)
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
                        .Include(x => x.Option).ThenInclude(o => o.Product).ThenInclude(x => x.ProductPhotos)
                        .OrderByDescending(x => x.DateCreated)
                        .AsNoTracking()
                        .ToListAsync();
        }
    }

    #endregion



}