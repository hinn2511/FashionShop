using API.Entities.OrderModel;
using API.Entities.ProductModel;
using API.Entities.User;

namespace API.Entities.UserModel
{
    public class UserReview : BaseEntity
    {
        public AppUser User { get; set; }
        public int UserId { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }
        public string Comment { get; set; }
        public int Score { get; set; }
    }
}