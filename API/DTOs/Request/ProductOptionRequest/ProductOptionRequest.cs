using API.DTOs.Request;

namespace API.DTOs.ProductOptionRequest
{
    public class ProductOptionRequest
    {
        public int ProductId { get; set; }
        public string SizeName { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public double AdditionalPrice { get; set; }
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