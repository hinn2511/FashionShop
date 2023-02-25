using System.Collections.Generic;
using API.Entities;
using API.Entities.WebPageModel;

namespace API.DTOs.Params
{
    public class CustomerReviewParams : PaginationParams
    {
        public int Score { get; set; }
    }
}