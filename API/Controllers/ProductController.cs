using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.Entities.ProductEntities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using static API.Extensions.StringExtensions;

namespace API.Controllers
{
    public class ProductController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerProductDto>>> GetProductsAsCustomer([FromQuery] ProductParams productParams)
        {

            var products = await _unitOfWork.ProductRepository.GetProductsAsCustomerAsync(productParams);

            Response.AddPaginationHeader(products.CurrentPage, products.PageSize, products.TotalCount, products.TotalPages);

            return Ok(products);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerProductDto>> GetProductAsCustomer(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductAsCustomerByIdAsync(id);
            return Ok(product);
        }

        [HttpPost("add")]
        public async Task<ActionResult<CustomerProductDto>> AddProduct(AddProductDto addProductDto)
        {
            var product = new Product();
            _mapper.Map(addProductDto, product);

            product.Slug = product.ProductName.GenerateSlug();
            product.Sold = 0;
            product.CreateAt = DateTime.UtcNow;

            _unitOfWork.ProductRepository.Add(product);

            if (await _unitOfWork.Complete()) 
            {
                var result = await _unitOfWork.ProductRepository.GetProductByIdAsync(product.Id);
                return Ok(result);
            }
            return BadRequest("Error when add product");
        }

        [HttpPut("edit")]
        public async Task<ActionResult<CustomerProductDto>> UpdateProduct(UpdateProductDto updateProductDto)
        {
            var product = await _unitOfWork.ProductRepository.FindProductByIdAsync(updateProductDto.Id);

            if(product == null)
                return BadRequest("Product not found");

            _mapper.Map(updateProductDto, product);

            product.Slug = updateProductDto.ProductName.GenerateSlug();

            _unitOfWork.ProductRepository.Update(product);

            if (await _unitOfWork.Complete()) 
            {
                var result = await _unitOfWork.ProductRepository.GetProductByIdAsync(product.Id);
                return Ok(result);
            }
            return BadRequest("Error when update product");
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.ProductRepository.FindProductByIdAsync(id);

            if(product == null)
                return BadRequest("Product not found"); 

            _unitOfWork.ProductRepository.Delete(product);

            if (await _unitOfWork.Complete()) 
            {
                return Ok();
            }
            return BadRequest("Error when delete product");
        }

    }
}