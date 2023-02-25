using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.Entities.Other;
using API.Helpers;
using API.Interfaces;
using API.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : GenericRepository<Photo>, IPhotoRepository
    {
        private readonly DataContext _context;

        public PhotoRepository(DataContext context, DbSet<Photo> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<PagedList<Photo>> GetImageAsync(PaginationParams paginationParams)
        {
            var query = _context.Photos.AsQueryable();

            if (paginationParams.OrderBy == OrderBy.Ascending)
            {
                query = query.OrderBy(x => x.DateCreated);
            }
            else
            {
                query = query.OrderByDescending(x => x.DateCreated);
            }
            return await PagedList<Photo>.CreateAsync(query.AsNoTracking(), paginationParams.PageNumber, paginationParams.PageSize);
        }
    }
}