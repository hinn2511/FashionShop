using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Params;
using API.Entities.WebPageModel;
using API.Extensions;
using API.Helpers;
using API.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.ConfigurationRepository
{
    public class HomePageRepository : GenericRepository<HomePage>, IHomePageRepository
    {
        private readonly DataContext _context;

        public HomePageRepository(DataContext context, DbSet<HomePage> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<HomePage> GetHomePageByIdAsync(int homePageId)
        {
            return await _context.HomePages
                                .Where(x => x.Id == homePageId)
                                .Include(x => x.Carousels)
                                .Include(x => x.FeatureCategories).ThenInclude(x => x.Category)
                                .Include(x => x.FeatureProducts).ThenInclude(x => x.Product)
                                .FirstOrDefaultAsync();
        }
    }

    public class CarouselRepository : GenericRepository<Carousel>, ICarouselRepository
    {
        private readonly DataContext _context;
        public CarouselRepository(DataContext context, DbSet<Carousel> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<PagedList<Carousel>> GetCarouselsAsync(CarouselParams carouselParams)
        {
            var query = _context.Carousels.AsQueryable();

            query = query.Where(x => carouselParams.CarouselStatus.Contains(x.Status));
            
            if (!string.IsNullOrEmpty(carouselParams.Query))
            {
                var words = carouselParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach (var word in words)
                {
                    query = query.Where(x => x.Title.ToUpper().Contains(word)
                        || x.Link.ToUpper().Contains(word));
                }
            }

            if (carouselParams.OrderBy == OrderBy.Ascending)
            {
                query = carouselParams.Field switch
                {
                    "date" => query.OrderBy(p => p.DateCreated),
                    "title" => query.OrderBy(p => p.Title),
                    "link" => query.OrderBy(p => p.Link),
                    "status" => query.OrderBy(p => p.Status),
                    _ => query.OrderBy(p => p.Id)
                };
            }
            else
            {
                query = carouselParams.Field switch
                {
                    "date" => query.OrderByDescending(p => p.DateCreated),
                    "title" => query.OrderByDescending(p => p.Title),
                    "link" => query.OrderByDescending(p => p.Link),
                    "status" => query.OrderByDescending(p => p.Status),
                    _ => query.OrderByDescending(p => p.Id)
                };
            }

            return await PagedList<Carousel>.CreateAsync(query, carouselParams.PageNumber, carouselParams.PageSize);
        }
    }

    public class FeatureProductRepository : GenericRepository<FeatureProduct>, IFeatureProductRepository
    {
        public FeatureProductRepository(DataContext context, DbSet<FeatureProduct> set) : base(context, set)
        {
        }
    }


    public class FeatureCategoryRepository : GenericRepository<FeatureCategory>, IFeatureCategoryRepository
    {
        public FeatureCategoryRepository(DataContext context, DbSet<FeatureCategory> set) : base(context, set)
        {
        }
    }
}

