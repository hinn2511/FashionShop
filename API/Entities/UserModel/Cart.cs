
using API.Entities.ProductModel;

namespace API.Entities.User
{
    public class Cart : BaseEntity
    {
        public AppUser User { get; set; }
        public int UserId { get; set; }
        public Option Option { get; set; }
        public int OptionId { get; set; }
        public int Quantity { get; set; }
        
    }
}