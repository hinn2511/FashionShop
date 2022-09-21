namespace API.DTOs.Request.Cart
{
    public class CartRequest
    {
        public int OptionId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartRequest : CartRequest
    {
    }

    
}