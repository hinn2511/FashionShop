using System.Threading.Tasks;
using API.Repository.BrandRepository;
using API.Repository.ConfigurationRepository;
using API.Repository.FileRepository;
using API.Repository.OrderRepository;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IProductRepository ProductRepository { get; }
        IPhotoRepository PhotoRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ISubCategoryRepository SubCategoryRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IOrderHistoryRepository OrderHistoryRepository { get; }
        IFileRepository FileRepository { get; }
        IBrandRepository BrandRepository { get; }
        IColorRepository ColorRepository { get; }
        ISizeRepository SizeRepository { get; }
        IStockRepository StockRepository { get; }
        IProductOptionRepository ProductOptionRepository { get; }
        IHomePageRepository HomePageRepository { get; }
        ICarouselRepository CarouselRepository { get; }
        IFeatureProductRepository FeatureProductRepository { get; }
        IFeatureCategoryRepository FeatureCategoryRepository { get; }
        Task<bool> Complete();
        bool HasChanges();

    }
}