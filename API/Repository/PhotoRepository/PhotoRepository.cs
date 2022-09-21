using System.Threading.Tasks;
using API.Entities.Other;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;
        public PhotoRepository(DataContext context)
        {
            _context = context;
        }

        public void Add(Photo photo)
        {
            _context.Photos.Add(photo);
        }

        public void Delete(Photo photo)
        {
            _context.Photos.Remove(photo);
        }

        public async Task<Photo> FindPhotoByIdAsync(int id)
        {
            return await _context.Photos.FirstOrDefaultAsync(p => p.Id == id); 
        }
    }
}