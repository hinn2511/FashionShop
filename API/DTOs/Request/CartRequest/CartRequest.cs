using System.Collections.Generic;

namespace API.DTOs.Request.CartRequest
{
    public class CartRequest
    {
        public int OptionId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateCartRequest : CartRequest
    {
    }


    public class UpdateCartRequest : CartRequest
    {
        public int CartId { get; set; }
    }

    public class UpdateCartAfterLoginRequest
    {
        public List<CartRequest> NewCartItems { get; set; }
    }

    
}