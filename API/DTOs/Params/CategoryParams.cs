using System.Collections.Generic;
using API.Entities;
using API.Entities.ProductModel;
using API.Entities.WebPageModel;

namespace API.DTOs.Params
{

    public class AdminCategoryParams : PaginationParams
    {
        public IList<Gender> Genders { get; set; } = new List<Gender>() { Gender.Men };

        public IList<Status> CategoryStatus { get; set; } = new List<Status>() { Status.Active };
    }
}