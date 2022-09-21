using System.Threading.Tasks;
using API.Entities;
using API.Entities.User;

namespace API.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);


    }
}