using System.Threading.Tasks;
using API.Entities.Other;
using API.Interfaces;
using API.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : GenericRepository<Photo>, IPhotoRepository
    {
        public PhotoRepository(DataContext context, DbSet<Photo> set) : base(context, set)
        {
        }
    }
}