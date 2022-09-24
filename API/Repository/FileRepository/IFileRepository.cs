using System.Threading.Tasks;
using API.Entities.OtherModel;
using API.Repository.GenericRepository;

namespace API.Repository.FileRepository
{
    public interface IFileRepository : IGenericRepository<UploadedFile>
    {
    }
}