using API.Entities.ProductModel;

namespace API.Entities.OrderModel
{
    public class OrderDetail : BaseEntity
    {
        public Order Order { get; set; }
        public int OrderId { get; set; }
        public Option Option { get; set; }
        public int OptionId { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; } 
        public int Quantity { get; set; }
    }
}