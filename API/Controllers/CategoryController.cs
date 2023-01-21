using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.DTOs.Request;
using API.DTOs.Request.CategoryRequest;
using API.DTOs.Response;
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
            var categories = await _unitOfWork.CategoryRepository.GetAllAndIncludeAsync(x => x.Status == Status.Active, "Parent", true);

            TreeExtension.ITree<Category> rootNode = categories.ToList().ToTree((parent, child) => child.ParentId == parent.Id);
            List<TreeExtension.ITree<Category>> rootLevelFoldersWithSubTree = rootNode.Children.ToList();

            var mappedData = _mapper.Map<IEnumerable<CustomerCategoryResponse>>(rootLevelFoldersWithSubTree);

            var result = mappedData.GroupBy(x => x.Gender).Select(x => new CustomerCategoryByGenderResponse
            {
                Gender = x.Key,
                GenderTitle = ((Gender)x.Key).ToString(),
                Categories = x.ToList()
            }).OrderBy(x => x.Gender);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("detail")]
        public async Task<ActionResult> GetCategoryAsCustomer([FromQuery] string slug, [FromQuery] Gender gender)
        {
            var category = await _unitOfWork.CategoryRepository.GetFirstByAndIncludeAsync(x => x.Slug == slug 
                && x.Gender == gender && x.Status == Status.Active, "Parent", true);

            if (category == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Category not found."));
            
            var result = _mapper.Map<CustomerSingleCategoryResponse>(category);

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


        [AllowAnonymous]
        [HttpGet("promoted")]
        public async Task<ActionResult> GetEditorChoiceArticlesAsCustomer()
        {

            var categories = await _unitOfWork.CategoryRepository.GetAllBy(x => x.IsPromoted && x.Status == Status.Active);

            var result = _mapper.Map<List<CustomerCategoryResponse>>(categories);

            return Ok(result);

        }

        #endregion


        #region manager
        [HttpGet("all")]
        public async Task<ActionResult> GetCategoriesAsAdmin([FromQuery] AdminCategoryParams adminCategoryParams)
        {
            var categories = await _unitOfWork.CategoryRepository.GetCategoriesAsync(adminCategoryParams);

            Response.AddPaginationHeader(categories.CurrentPage, categories.PageSize, categories.TotalCount, categories.TotalPages);

            return Ok(_mapper.Map<List<AdminCategoryResponse>>(categories.ToList()));
        }

        [HttpGet("detail/{id}")]
        public async Task<ActionResult> GetCategoryDetailAsAdmin(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetFirstBy(x => x.Id == id);
            return Ok(_mapper.Map<AdminCategoryDetailResponse>(category));
        }

        [HttpGet("catalogue")]
        public async Task<ActionResult> GetCatalogueAsAdmin()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAndIncludeAsync(x => x.Status == Status.Active, "Parent", true);

            TreeExtension.ITree<Category> rootNode = categories.ToList().ToTree((parent, child) => child.ParentId == parent.Id);
            List<TreeExtension.ITree<Category>> rootLevelFoldersWithSubTree = rootNode.Children.ToList();

            var mappedData = _mapper.Map<IEnumerable<AdminCatalogueCategoryResponse>>(rootLevelFoldersWithSubTree);

            var result = mappedData.GroupBy(x => x.Gender).Select(x => new AdminCatalogueResponse
            {
                Gender = x.Key,
                GenderTitle = ((Gender)x.Key).ToString(),
                Categories = x.ToList()
            }).OrderBy(x => x.Gender);

            return Ok(result);
        }


        [HttpPost("create")]
        public async Task<ActionResult> AddCategory(CreateCategoryRequest createCategoryRequest)
        {
            var category = new Category();

            _mapper.Map(createCategoryRequest, category);

            if (createCategoryRequest.ParentId > 0)
            {
                var parentCategory = await _unitOfWork.CategoryRepository.GetFirstBy(x => x.Id == createCategoryRequest.ParentId);

                if (parentCategory == null)
                    return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Parent category not found."));
                category.Gender = parentCategory.Gender;
            }
            else
                category.ParentId = null;

            category.Slug = category.CategoryName.GenerateSlug();

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

            if (updateCategoryRequest.ParentId > 0)
            {
                var parentCategory = await _unitOfWork.CategoryRepository.GetFirstBy(x => x.Id == updateCategoryRequest.ParentId);

                if (parentCategory == null)
                    return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Parent category not found."));
                category.Gender = parentCategory.Gender;
            }
            else
                category.ParentId = null;

            category.Id = id;
            category.Slug = updateCategoryRequest.CategoryName.GenerateSlug();
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
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Category not found"));

            foreach (var category in categories)
            {
                if (category.Status == Status.Deleted)

                    continue;

                var subCategories = await _unitOfWork.CategoryRepository.GetAllBy(x => x.ParentId == category.Id);
                foreach (var subCategory in subCategories)
                {
                     if (subCategory.Status == Status.Deleted)

                    continue;
                    category.AddDeleteInformation(GetUserId());

                }
                category.AddDeleteInformation(GetUserId());
            }

            _unitOfWork.CategoryRepository.Update(categories);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while deleting categories."));
        }

        [HttpDelete("hard-delete")]
        public async Task<ActionResult> HardDeleteCategory(DeleteCategoriesRequest deleteCategoriesRequest)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllBy(x => deleteCategoriesRequest.Ids.Contains(x.Id));

            if (categories == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Category not found"));

            _unitOfWork.CategoryRepository.Delete(categories);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while deleting categories."));
        }

        [HttpPut("hide")]
        public async Task<ActionResult> HidingCategory(HideCategoriesRequest hideCategoriesRequest)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllBy(x => hideCategoriesRequest.Ids.Contains(x.Id) && x.Status == Status.Active);

            if (categories == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Category not found"));

            foreach (var category in categories)
            {
                category.AddHiddenInformation(GetUserId());

            }

            _unitOfWork.CategoryRepository.Update(categories);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Successfully hide {categories.Count()} category(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while active categories."));
        }

        [HttpPut("activate")]
        public async Task<ActionResult> ActiveCategory(HideCategoriesRequest hideCategoriesRequest)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllBy(x => hideCategoriesRequest.Ids.Contains(x.Id) && x.Status == Status.Hidden);

            if (categories == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Category not found"));

            foreach (var category in categories)
            {
                category.Status = Status.Active;
                category.AddUpdateInformation(GetUserId());
            }

            _unitOfWork.CategoryRepository.Update(categories);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Successfully unhide {categories.Count()} category(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while hiding categories."));
        }

        [HttpPut("demote")]
        public async Task<ActionResult> RemoveEditorChoiceForCategory(BulkDemoteRequest bulkDemoteRequest)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllBy(x => bulkDemoteRequest.Ids.Contains(x.Id));

            if (categories == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Category not found"));

            foreach (var category in categories)
            {
                if (category.IsPromoted)
                    category.IsPromoted = false;
            }

            _unitOfWork.CategoryRepository.Update(categories);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Successfully demote for {categories.Count()} category(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while demote for categories."));
        }

        [HttpPut("promote")]
        public async Task<ActionResult> SetEditorChoiceForCategory(BulkPromoteRequest bulkPromoteRequest)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllBy(x => bulkPromoteRequest.Ids.Contains(x.Id));

            if (categories == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Category not found"));

            foreach (var category in categories)
            {
                if (!category.IsPromoted)
                    category.IsPromoted = true;
            }

            _unitOfWork.CategoryRepository.Update(categories);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Successfully promote for {categories.Count()} category(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while promote for categories."));
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