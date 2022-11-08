using System.Collections.Generic;
using API.Entities;

namespace API.DTOs.Params
{
    public class CarouselParams : PaginationParams
    {
        public IList<Status> CarouselStatus { get; set; } = new List<Status>() { Status.Active };
    }
}