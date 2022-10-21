using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
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

        public IUserLikeRepository UserLikeRepository => new UserLikeRepository(_context, _context.UserLikes);
        public ICartRepository CartRepository => new CartRepository(_context, _context.Carts);
        public IProductRepository ProductRepository => new ProductRepository(_context,  _context.Products);
        public ICategoryRepository CategoryRepository => new CategoryRepository(_context, _context.Categories);
        public ISubCategoryRepository SubCategoryRepository => new SubCategoryRepository(_context, _context.SubCategories);
        public IProductPhotoRepository ProductPhotoRepository => new ProductPhotoRepository(_context, _context.ProductPhotos);
        public IOrderRepository OrderRepository => new OrderRepository(_context, _context.Orders);
        public IOrderDetailRepository OrderDetailRepository => new OrderDetailRepository(_context, _context.OrderDetails);
        public IOrderHistoryRepository OrderHistoryRepository => new OrderHistoryRepository(_context, _context.OrderHistories);
        public IBrandRepository BrandRepository => new BrandRepository(_context, _context.Brands);
        public IColorRepository ColorRepository => new ColorRepository(_context, _context.Colors);
        public IFileRepository FileRepository => new FileRepository(_context, _context.Files);
        public ISizeRepository SizeRepository => new SizeRepository(_context, _context.Sizes);
        public IStockRepository StockRepository => new StockRepository(_context, _context.Stocks);
        public IProductOptionRepository ProductOptionRepository => new ProductOptionRepository(_context, _context.Options);
        public IHomePageRepository HomePageRepository => new HomePageRepository(_context, _context.HomePages);
        public ICarouselRepository CarouselRepository => new CarouselRepository(_context, _context.Carousels);
        public IFeatureProductRepository FeatureProductRepository => new FeatureProductRepository(_context, _context.FeatureProducts);
        public IFeatureCategoryRepository FeatureCategoryRepository => new FeatureCategoryRepository(_context, _context.FeatureCategories);

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}