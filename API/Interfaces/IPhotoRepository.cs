using System.Threading.Tasks;
using API.Entities.Other;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<Photo> FindPhotoByIdAsync(int id);
        void Add(Photo photo);
        void Delete(Photo photo);
    }
}