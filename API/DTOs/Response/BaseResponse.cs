using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Response
{
    public class BaseResponse
    {
        public int Id { get; set; }
    }

    public class BasePhotoResponse
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
    }
    
}