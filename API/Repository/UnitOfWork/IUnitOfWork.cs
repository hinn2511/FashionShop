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
        ISettingsRepository SettingsRepository { get; }
        IAppRoleRepository AppRoleRepository { get; }
        IAppRolePermissionRepository  AppRolePermissionRepository { get; }
        IUserLikeRepository UserLikeRepository { get; }
        IUserReviewRepository UserReviewRepository { get; }
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
        IProductOptionRepository ProductOptionRepository { get; }
        IHomePageRepository HomePageRepository { get; }
        ICarouselRepository CarouselRepository { get; }
        IFeatureProductRepository FeatureProductRepository { get; }
        IFeatureCategoryRepository FeatureCategoryRepository { get; }
        IArticleRepository ArticleRepository { get; }
        Task<bool> Complete();
        bool HasChanged();

    }
}