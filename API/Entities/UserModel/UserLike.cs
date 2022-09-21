using API.Entities.ProductModel;
using API.Entities.User;

namespace API.Entities.UserModel
{
    public class UserLike : BaseEntity
    {
         public AppUser User { get; set; }
         public int UserId { get; set; }
         public Product Product { get; set; }
         public int ProductId { get; set; }
    }
}