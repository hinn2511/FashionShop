using System.Threading.Tasks;
using API.Data;
using API.Entities.OtherModel;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.FileRepository
{
    public class FileRepository : IFileRepository
    {
        private readonly DataContext _context;

        public FileRepository(DataContext context)
        {
            _context = context;
        }

        public void Create(UploadedFile uploadedFile)
        {
            _context.Add(uploadedFile);
        }

        public async Task<UploadedFile> GetById(int id)
        {
            return await _context.Files.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}