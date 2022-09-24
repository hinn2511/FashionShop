using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities.WebPageModel;
using API.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.ConfigurationRepository
{
    public class HomePageRepository : GenericRepository<HomePage>, IHomePageRepository
    {
        public HomePageRepository(DataContext context, DbSet<HomePage> set) : base(context, set)
        {
        }
    }

    public class CarouselRepository : GenericRepository<Carousel>, ICarouselRepository
    {
        public CarouselRepository(DataContext context, DbSet<Carousel> set) : base(context, set)
        {
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

