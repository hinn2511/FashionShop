namespace API.DTOs.Params
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = ( value > MaxPageSize ) ? MaxPageSize : value;
        }

        public OrderBy OrderBy { get; set; } = OrderBy.Ascending;
        public string Field { get; set; } = "Id";
        public string Query { get; set; } = "";
    }

    public enum OrderBy 
    {
        Ascending ,
        Descending
    }
}