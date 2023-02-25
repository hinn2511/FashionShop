using System.Threading.Tasks;
using API.DTOs.Params;
using API.Entities.Other;
using API.Helpers;
using API.Repository.GenericRepository;

namespace API.Interfaces
{
    public interface IPhotoRepository : IGenericRepository<Photo>
    {
        Task<PagedList<Photo>> GetImageAsync(PaginationParams paginationParams);
    }
}