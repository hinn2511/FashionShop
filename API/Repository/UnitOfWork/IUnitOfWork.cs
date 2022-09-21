using System.Threading.Tasks;
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
        IOrderRepository OrderRepository { get; }
        IFileRepository FileRepository { get; }
        IConfigurationRepository ConfigurationRepository { get; }
        Task<bool> Complete();
        bool HasChanges();

    }
}