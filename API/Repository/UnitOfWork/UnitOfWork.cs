using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
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

        public IUserRepository UserRepository => new UserRepository(_context, _mapper);
        public IProductRepository ProductRepository => new ProductRepository(_context, _mapper);
        public ICategoryRepository CategoryRepository => new CategoryRepository(_context, _mapper);
        public IPhotoRepository PhotoRepository => new PhotoRepository(_context);
        public IOrderRepository OrderRepository => new OrderRepository(_context);
        public IFileRepository FileRepository => new FileRepository(_context);
        public IConfigurationRepository ConfigurationRepository => new ConfigurationRepository(_context);
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