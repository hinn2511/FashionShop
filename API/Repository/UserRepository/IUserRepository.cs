using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Entities.User;
using API.Entities.UserModel;
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
    }


    #endregion

    #region user cart
    public interface ICartRepository : IGenericRepository<Cart>
    {
    }

    #endregion
}