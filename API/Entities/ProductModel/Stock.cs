namespace API.Entities.ProductModel
{
    public class Stock : BaseEntity
    {
        public Option Option { get; set; }
        public int OptionId { get; set; }
        public int Quantity { get; set; }
    }
}