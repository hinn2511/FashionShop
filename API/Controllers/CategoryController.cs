using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.DTOs.Request.CategoryRequest;
using API.DTOs.Response.CategoryResponse;
using API.Entities;
using API.Entities.ProductModel;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class CategoryController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region customer
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetCategoriesAsCustomer()
        {
            var categories = await _unitOfWork.CategoryRepository.GetCategoriesWithSubCategoriesAsync();
            FilterCategories(categories);

            var result = categories.GroupBy(x => x.Gender).Select(x => new CategoryByGenderResponse
            {
                Gender = x.Key,
                GenderTitle = ((Gender) x.Key).ToString(),
                Categories = _mapper.Map<List<CategoryGender>>(x.OrderBy(x => x.CategoryName))
            });

            return Ok(result);

        }

        [AllowAnonymous]
        [HttpGet("filtered-by-products")]
        public async Task<ActionResult> GetCategoriesByCategoryFilterAsCustomer([FromQuery] CustomerProductParams customerProductParams)
        {
            var categories = await _unitOfWork.CategoryRepository.GetCategoriesByProductFilterAsync(customerProductParams);
            FilterCategories(categories);
            return Ok(_mapper.Map<CustomerCategoryResponse>(categories));
        }
        #endregion


        #region manager
        [HttpGet("all")]
        public async Task<ActionResult> GetCategoriesAsAdmin()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAll();
            return Ok(_mapper.Map<IEnumerable<AdminCategoryResponse>>(categories));
        }

        [HttpGet("detail/{id}")]
        public async Task<ActionResult> GetCategoryDetailAsAdmin(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetFirstByAndIncludeAsync(x => x.Id == id, "SubCategories", true);
            return Ok(_mapper.Map<AdminCategoryDetailResponse>(category));
        }


        [HttpPost("create")]
        public async Task<ActionResult> AddCategory(CreateCategoryRequest createCategoryRequest)
        {
            var category = new Category();
            _mapper.Map(createCategoryRequest, category);

            //category.Slug = category.CategoryName.GenerateSlug();
            category.AddCreateInformation(GetUserId());

            _unitOfWork.CategoryRepository.Insert(category);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while adding the category.");
        }

        [HttpPut("edit/{id}")]
        public async Task<ActionResult> UpdateCategory(int id, UpdateCategoryRequest updateCategoryRequest)
        {
            var category = await _unitOfWork.CategoryRepository.GetById(id);

            if (category == null)
                return BadRequest("Category not found");

            _mapper.Map(updateCategoryRequest, category);

            category.Id = id;
            //category.Slug = updateCategoryRequest.CategoryName.GenerateSlug();
            category.AddUpdateInformation(GetUserId());

            _unitOfWork.CategoryRepository.Update(category);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while updating the category.");
        }

        [HttpDelete("soft-delete")]
        public async Task<ActionResult> SoftDeleteCategory(DeleteCategoriesRequest deleteCategoriesRequest)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllBy(x => deleteCategoriesRequest.Ids.Contains(x.Id));

            if (categories == null)
                return BadRequest("Category not found");

            foreach (var category in categories)
            {
                category.AddDeleteInformation(GetUserId());
            }

            if (deleteCategoriesRequest.IncludeProducts)
            {
                var products = await _unitOfWork.ProductRepository.GetAllBy(x => deleteCategoriesRequest.Ids.Contains(x.CategoryId));
                foreach(var product in products)
                    product.AddDeleteInformation(GetUserId());
                _unitOfWork.ProductRepository.Update(products);
            }

            _unitOfWork.CategoryRepository.Update(categories);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while deleting categories.");
        }

        [HttpDelete("hard-delete")]
        public async Task<ActionResult> HardDeleteCategory(DeleteCategoriesRequest deleteCategoriesRequest)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllBy(x => deleteCategoriesRequest.Ids.Contains(x.Id));

            if (categories == null)
                return BadRequest("Category not found");

            _unitOfWork.CategoryRepository.Delete(categories);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while deleting categories.");
        }

        [HttpPut("hide")]
        public async Task<ActionResult> HidingCategory(HideCategoriesRequest hideCategoriesRequest)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllBy(x => hideCategoriesRequest.Ids.Contains(x.Id));

            if (categories == null)
                return BadRequest("Category not found");

            foreach (var category in categories)
            {
                category.AddHiddenInformation(GetUserId());
            }

            if (hideCategoriesRequest.IncludeProducts)
            {
                var products = await _unitOfWork.ProductRepository.GetAllBy(x => hideCategoriesRequest.Ids.Contains(x.CategoryId));
                foreach(var product in products)
                {
                    if(product.Status == Status.Active)
                        product.AddHiddenInformation(GetUserId());
                }
                _unitOfWork.ProductRepository.Update(products);
            }

            _unitOfWork.CategoryRepository.Update(categories);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while hiding categories.");
        }

        [HttpPut("unhide")]
        public async Task<ActionResult> UndoHidingCategory(HideCategoriesRequest hideCategoriesRequest)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllBy(x => hideCategoriesRequest.Ids.Contains(x.Id));

            if (categories == null)
                return BadRequest("Category not found");

            foreach (var category in categories)
            {
                category.Status = Status.Active;
            }

            if (hideCategoriesRequest.IncludeProducts)
            {
                var products = await _unitOfWork.ProductRepository.GetAllBy(x => hideCategoriesRequest.Ids.Contains(x.CategoryId));
                foreach(var product in products)
                {
                    if(product.Status == Status.Hidden)
                        product.Status = Status.Active;
                }
                _unitOfWork.ProductRepository.Update(products);
            }

            _unitOfWork.CategoryRepository.Update(categories);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while undo hiding categories.");
        }

        [HttpPost("{categoryId}/create/sub-category")]
        public async Task<ActionResult> AddSubCategory(int categoryId, CreateCategoryRequest createCategoryRequest)
        {
            var category = await _unitOfWork.CategoryRepository.GetFirstBy(x => x.Id == categoryId);

            if (category == null)
                return BadRequest("Category not found");

            var subCategory = new SubCategory();
            
            _mapper.Map(createCategoryRequest, subCategory);

            //category.Slug = category.CategoryName.GenerateSlug();
            subCategory.AddCreateInformation(GetUserId());
            subCategory.CategoryId = categoryId;

            _unitOfWork.SubCategoryRepository.Insert(subCategory);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while adding the sub category.");
        }



        #endregion


        #region private method
        private void FilterCategories(IEnumerable<Category> categories)
        {
            var filteredCategories = categories.ToList();
            foreach (var category in filteredCategories)
            {
                if (category.Status == Status.Deleted || category.Status == Status.Hidden)
                {
                    filteredCategories.Remove(category);
                    continue;
                }

                foreach (var subCategory in category.SubCategories)
                {
                    if (subCategory.Status == Status.Deleted || subCategory.Status == Status.Hidden)
                    {
                        category.SubCategories.Remove(subCategory);
                    }
                }

            }
        }

        #endregion
    }
}