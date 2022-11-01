using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.ProductModel;

namespace API.DTOs.Response.CategoryResponse
{
    public class CategoryByGenderResponse
    {
        public Gender Gender { get; set; }
        public string GenderTitle { get; set; }
        public List<CategoryGender> Categories { get; set; }
    }

    public class CategoryGender
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public List<SubCategoryGender> SubCategories { get; set; }
    }

    public class SubCategoryGender
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
    }
}