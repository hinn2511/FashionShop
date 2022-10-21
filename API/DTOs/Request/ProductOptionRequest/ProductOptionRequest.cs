using API.DTOs.Request;

namespace API.DTOs.ProductOptionRequest
{
    public class ProductOptionRequest
    {
        public int SizeId { get; set; }
        public int ColorId { get; set; }
    }

    public class CreateProductOptionRequest : ProductOptionRequest
    {
    }

    public class UpdateProductOptionRequest : ProductOptionRequest
    {
    }

    public class HideProductOptionsRequest : BaseBulkRequest
    {
    }

    public class DeleteProductOptionsRequest : BaseBulkRequest
    {
    }
}