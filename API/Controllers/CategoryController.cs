using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.Entities.ProductModel;
using API.Interfaces;
using AutoMapper;
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
        
        [HttpGet("{gender}")]
        public async Task<ActionResult<IEnumerable<CustomerCategoryDto>>> GetCategoryAsCustomer(Gender gender)
        {

            var categories = await _unitOfWork.CategoryRepository.GetCategoriesAsCustomerAsync(gender);
            return Ok(categories);

        }

        [HttpPost("add")]
        public async Task<ActionResult<CustomerCategoryDto>> AddCategory(AddCategoryDto addCategoryDto)
        {
            var category = new Category();
            _mapper.Map(addCategoryDto, category);
            _unitOfWork.CategoryRepository.Add(category);
            if (await _unitOfWork.Complete()) 
            {
                var result = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(category.Id);
                return Ok(result);
            }
            return BadRequest("Error when add category");
        }

        [HttpPut("edit")]
        public async Task<ActionResult<Category>> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            var category = await _unitOfWork.CategoryRepository.FindCategoryByIdAsync(updateCategoryDto.Id);

            if(category == null)
                return BadRequest("Category not found");

            _mapper.Map(updateCategoryDto, category);


            _unitOfWork.CategoryRepository.Update(category);
            if (await _unitOfWork.Complete()) 
            {
                var result = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(category.Id);
                return Ok(result);
            }
            return BadRequest("Error when update category");
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var category = await _unitOfWork.CategoryRepository.FindCategoryByIdAsync(id);

            if(category == null)
                return BadRequest("Category not found"); 

            _unitOfWork.CategoryRepository.Delete(category);
            if (await _unitOfWork.Complete()) 
            {
                return Ok();
            }
            return BadRequest("Error when delete category");
        }
    }
}