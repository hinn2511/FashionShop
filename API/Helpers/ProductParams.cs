namespace API.Helpers
{
    public class ProductParams : PaginationParams
    {
        public string Category { get; set; }
        public string Gender { get; set; }
        public string OrderBy { get; set; } = "newest";
    }
}