using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.Entities.WebPageModel;
using API.Helpers;
using API.Repository.GenericRepository;

namespace API.Repository.ConfigurationRepository
{
     public interface IHomePageRepository : IGenericRepository<HomePage>
    {
        Task<HomePage> GetHomePageByIdAsync(int homePageId);
    }

    public interface ICarouselRepository : IGenericRepository<Carousel>
    {
        Task<PagedList<Carousel>> GetCarouselsAsync(CarouselParams carouselParams);
    }

    public interface IFeatureProductRepository : IGenericRepository<FeatureProduct>
    {
    }

    public interface IFeatureCategoryRepository : IGenericRepository<FeatureCategory>
    {
    }

    public interface ISettingsRepository : IGenericRepository<Settings>
    {
    }

     
}