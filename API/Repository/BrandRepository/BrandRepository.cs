using API.Data;
using API.Entities.ProductModel;
using API.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.BrandRepository
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(DataContext context, DbSet<Brand> set) : base(context, set)
        {
        }
    }
}