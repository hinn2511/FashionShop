using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IProductRepository ProductRepository { get; }
        IPhotoRepository PhotoRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        Task<bool> Complete();
        bool HasChanges();

    }
}