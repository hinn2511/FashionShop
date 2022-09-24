using System.Threading.Tasks;
using API.Data;
using API.Entities.OtherModel;
using API.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.FileRepository
{
    public class FileRepository : GenericRepository<UploadedFile>, IFileRepository
    {
        public FileRepository(DataContext context, DbSet<UploadedFile> set) : base(context, set)
        {
        }
    }
}