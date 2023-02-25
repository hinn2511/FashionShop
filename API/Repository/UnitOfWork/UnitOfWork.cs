using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using API.Repository.ArticleRepository;
using API.Repository.BrandRepository;
using API.Repository.ConfigurationRepository;
using API.Repository.FileRepository;
using API.Repository.OrderRepository;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public ISettingsRepository  SettingsRepository => new SettingsRepository(_context, _context.Settings);
        public IAppRoleRepository AppRoleRepository => new AppRoleRepository(_context, _context.AppRoles);
        public IAppRolePermissionRepository AppRolePermissionRepository => new AppRolePermissionRepository(_context, _context.RolePermissions);
        public IUserLikeRepository UserLikeRepository => new UserLikeRepository(_context, _context.UserLikes);
        public IUserReviewRepository UserReviewRepository => new UserReviewRepository(_context, _context.UserReviews);
        public ICartRepository CartRepository => new CartRepository(_context, _context.Carts);
        public IProductRepository ProductRepository => new ProductRepository(_context,  _context.Products);
        public ICategoryRepository CategoryRepository => new CategoryRepository(_context, _context.Categories);
        public IProductPhotoRepository ProductPhotoRepository => new ProductPhotoRepository(_context, _context.ProductPhotos);
        public IOrderRepository OrderRepository => new OrderRepository(_context, _context.Orders);
        public IOrderDetailRepository OrderDetailRepository => new OrderDetailRepository(_context, _context.OrderDetails);
        public IOrderHistoryRepository OrderHistoryRepository => new OrderHistoryRepository(_context, _context.OrderHistories);
        public IBrandRepository BrandRepository => new BrandRepository(_context, _context.Brands);
        public IFileRepository FileRepository => new FileRepository(_context, _context.Files);
        public IProductOptionRepository ProductOptionRepository => new ProductOptionRepository(_context, _context.Options);
        public IHomePageRepository HomePageRepository => new HomePageRepository(_context, _context.HomePages);
        public ICarouselRepository CarouselRepository => new CarouselRepository(_context, _context.Carousels);
        public IFeatureProductRepository FeatureProductRepository => new FeatureProductRepository(_context, _context.FeatureProducts);
        public IFeatureCategoryRepository FeatureCategoryRepository => new FeatureCategoryRepository(_context, _context.FeatureCategories);
        public IArticleRepository ArticleRepository => new ArticleRepository(_context, _context.Articles);
        public IPhotoRepository PhotoRepository => new PhotoRepository(_context, _context.Photos);

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanged()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}