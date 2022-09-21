using System.Threading.Tasks;
using API.Entities.OtherModel;

namespace API.Repository.FileRepository
{
    public interface IFileRepository
    {
        void Create (UploadedFile uploadedFile);
        Task<UploadedFile> GetById(int id);
    }
}