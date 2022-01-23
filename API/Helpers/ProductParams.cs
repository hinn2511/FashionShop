namespace API.Helpers
{
    public class ProductParams : PaginationParams
    {
        public string OrderBy { get; set; } = "newest";
    }
}