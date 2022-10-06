using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.DTOs.Request.CategoryRequest;
using API.Entities.ProductModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    
    public class CategoryController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        
        #region category
        [HttpGet]
        public async Task<ActionResult> GetCategoryAsCustomer()
        {
            return Ok(await _unitOfWork.CategoryRepository.GetAll());
        }

        [HttpPost]
        public async Task<ActionResult<CustomerCategoryDto>> AddCategory(CategoryRequest categoryRequest)
        {
            var category = _mapper.Map<Category>(categoryRequest);

            category.CreatedByUserId = User.GetUserId();
            category.DateCreated = DateTime.UtcNow;

            _unitOfWork.CategoryRepository.Insert(category);

            if (await _unitOfWork.Complete()) 
            {
                return Ok();
            }
            return BadRequest("Error when add category");
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<Category>> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            var category = await _unitOfWork.CategoryRepository.GetById(updateCategoryDto.Id);

            if(category == null)
                return BadRequest("Category not found");

            _mapper.Map(updateCategoryDto, category);


            _unitOfWork.CategoryRepository.Update(category);
            if (await _unitOfWork.Complete()) 
            {
                var result = await _unitOfWork.CategoryRepository.GetById(category.Id);
                return Ok(result);
            }
            return BadRequest("Error when update category");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetById(id);

            if(category == null)
                return BadRequest("Category not found"); 

            _unitOfWork.CategoryRepository.Delete(category);
            if (await _unitOfWork.Complete()) 
            {
                return Ok();
            }
            return BadRequest("Error when delete category");
        }
    
        #endregion


        #region sub category
        
        [HttpGet("{id}/subCategories")]
        public async Task<ActionResult> GetSubCategoriesAsCustomer(int id)
        {
            return Ok(await _unitOfWork.SubCategoryRepository.GetAllBy(x => x.CategoryId == id));
        }


        [HttpPost("subCategory")]
        public async Task<ActionResult<CustomerCategoryDto>> AddSubCategory(SubCategoryRequest subCategoryRequest)
        {

            if (await _unitOfWork.CategoryRepository.GetById(subCategoryRequest.ParentCategoryId) == null)
                return BadRequest("Parent category not found");

            var subCategory = _mapper.Map<SubCategory>(subCategoryRequest);

            subCategory.CreatedByUserId = User.GetUserId();
            subCategory.DateCreated = DateTime.UtcNow;

            _unitOfWork.SubCategoryRepository.Insert(subCategory);

            if (await _unitOfWork.Complete()) 
            {
                return Ok();
            }
            return BadRequest("Error when add sub category");
        }

        [HttpPut("subCategory")]
        public async Task<ActionResult<Category>> UpdateSubCategory(UpdateSubCategoryRequest updateSubCategoryRequest)
        {
            if (await _unitOfWork.SubCategoryRepository.GetById(updateSubCategoryRequest.ParentCategoryId) == null)
                return BadRequest("Parent category not found");

            var subCategory = await _unitOfWork.SubCategoryRepository.GetById(updateSubCategoryRequest.Id);

            if(subCategory == null)
                return BadRequest("Sub category not found");


            _unitOfWork.SubCategoryRepository.Update(_mapper.Map<SubCategory>(updateSubCategoryRequest));

            if (await _unitOfWork.Complete()) 
            {
                return Ok();
            }
            return BadRequest("Error when update sub category");
        }

        [HttpDelete("subCategory/{subCategoryId}")]
        public async Task<ActionResult> DeleteSubCategoryCategory(int subCategoryId)
        {
             var subCategory = await _unitOfWork.SubCategoryRepository.GetById(subCategoryId);

            if(subCategory == null)
                return BadRequest("Sub category not found");

            _unitOfWork.SubCategoryRepository.Delete(subCategory);

            if (await _unitOfWork.Complete()) 
            {
                return Ok();
            }

            return BadRequest("Error when delete sub category");
        }
    

        #endregion
    }
}