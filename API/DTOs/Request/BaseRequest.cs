using System.Collections.Generic;

namespace API.DTOs.Request
{
    public class BaseRequest
    {
        
    }

    public class BaseBulkRequest
    {
        public IList<int> Ids { get; set; }
    }
}