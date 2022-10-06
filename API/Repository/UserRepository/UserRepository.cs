using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Entities.User;
using API.Entities.UserModel;
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
        public UserLikeRepository(DataContext context, DbSet<UserLike> set) : base(context, set)
        {
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


    #region user like
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(DataContext context, DbSet<Cart> set) : base(context, set)
        {
        }
    }

    #endregion



}