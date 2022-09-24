using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities.WebPageModel;
using API.Repository.GenericRepository;

namespace API.Repository.ConfigurationRepository
{
    public interface IHomePageRepository : IGenericRepository<HomePage>
    {
    }

    public interface ICarouselRepository : IGenericRepository<Carousel>
    {
    }

    public interface IFeatureProductRepository : IGenericRepository<FeatureProduct>
    {
    }

    public interface IFeatureCategoryRepository : IGenericRepository<FeatureCategory>
    {
    }

     
}