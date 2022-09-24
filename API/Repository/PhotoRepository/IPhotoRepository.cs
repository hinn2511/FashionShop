using System.Threading.Tasks;
using API.Entities.Other;
using API.Repository.GenericRepository;

namespace API.Interfaces
{
    public interface IPhotoRepository : IGenericRepository<Photo>
    {
    }
}