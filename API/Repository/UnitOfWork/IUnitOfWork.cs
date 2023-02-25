using System.Threading.Tasks;
using API.Repository.ArticleRepository;
using API.Repository.BrandRepository;
using API.Repository.ConfigurationRepository;
using API.Repository.FileRepository;
using API.Repository.OrderRepository;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserLikeRepository UserLikeRepository { get; }
        ICartRepository CartRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductPhotoRepository ProductPhotoRepository { get; }
        IPhotoRepository PhotoRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IOrderHistoryRepository OrderHistoryRepository { get; }
        IFileRepository FileRepository { get; }
        IBrandRepository BrandRepository { get; }
        IStockRepository StockRepository { get; }
        IProductOptionRepository ProductOptionRepository { get; }
        IHomePageRepository HomePageRepository { get; }
        ICarouselRepository CarouselRepository { get; }
        IFeatureProductRepository FeatureProductRepository { get; }
        IFeatureCategoryRepository FeatureCategoryRepository { get; }
        IArticleRepository ArticleRepository { get; }
        Task<bool> Complete();
        bool HasChanges();

    }
}