using API.Entities.ProductModel;

namespace API.Entities.OrderModel
{
    public class OrderDetail : BaseEntity
    {
        public Order Order { get; set; }
        public int OrderId { get; set; }
        public Option Option { get; set; }
        public int OptionId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
        public bool IsReviewed { get; set; }
        public int ReviewEditCount { get; set; }
    }
}