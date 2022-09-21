using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.Entities.Other;
using API.Entities.ProductModel;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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

        #region product API
        [HttpPost("add")]
        public async Task<ActionResult<CustomerProductDto>> AddProduct(AddProductDto addProductDto)
        {
            var product = new Product();
            _mapper.Map(addProductDto, product);

            product.Slug = product.ProductName.GenerateSlug();
            product.Sold = 0;
            product.DateCreated = DateTime.UtcNow;
            product.CreatedByUserId = User.GetUserId();

            _unitOfWork.ProductRepository.Add(product);

            if (await _unitOfWork.Complete())
            {
                var result = await _unitOfWork.ProductRepository.GetProductByIdAsync(product.Id);
                return Ok(result);
            }
            return BadRequest("An error occurred while adding the product.");
        }

        [HttpPut("edit")]
        public async Task<ActionResult<CustomerProductDto>> UpdateProduct(UpdateProductDto updateProductDto)
        {
            var product = await _unitOfWork.ProductRepository.GetProductByIdAsync(updateProductDto.Id);

            if (product == null)
                return BadRequest("Product not found");

            _mapper.Map(updateProductDto, product);

            product.Slug = updateProductDto.ProductName.GenerateSlug();

            _unitOfWork.ProductRepository.Update(product);

            if (await _unitOfWork.Complete())
            {
                var result = await _unitOfWork.ProductRepository.GetProductByIdAsync(product.Id);
                return Ok(result);
            }
            return BadRequest("An error occurred while updating the product.");
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductByIdAsync(id);

            if (product == null)
                return BadRequest("Product not found");

            _unitOfWork.ProductRepository.Delete(product);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while deleting the product.");
        }

        [HttpPost("add-product-photo/{productId}")]
        public async Task<ActionResult<ProductPhotoDto>> AddProductPhoto(IFormFile file, int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetProductByIdAsync(productId);
            var result = await _photoService.AddPhotoAsync(file, 700, 700);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            _unitOfWork.PhotoRepository.Add(photo);

            if (await _unitOfWork.Complete())
            {
                var productPhoto = new ProductPhoto
                {
                    ProductId = productId,
                    PhotoId = photo.Id,
                    IsMain = product.ProductPhotos.Count == 0 ? true : false
                };

                product.ProductPhotos.Add(productPhoto);

                if (await _unitOfWork.Complete())
                {
                    return _mapper.Map<ProductPhotoDto>(productPhoto);
                }
            }

            return BadRequest("An error occurred while adding the image.");
        }

        [HttpPut("set-main-product-photo/{productId}/{productPhotoId}")]
        public async Task<ActionResult> SetMainProductPhoto(int productId, int productPhotoId)
        {
            var product = await _unitOfWork.ProductRepository.GetProductByIdAsync(productId);

            var productPhoto = product.ProductPhotos.FirstOrDefault(x => x.Id == productPhotoId);

            if (productPhoto.IsMain) return BadRequest("This image is already main image.");

            var currentMain = product.ProductPhotos.FirstOrDefault(x => x.IsMain);

            if (currentMain != null) currentMain.IsMain = false;

            productPhoto.IsMain = true;

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("An error occurred while setting the main image.");
        }

        [HttpDelete("delete-product-photo/{productId}/{productPhotoId}")]
        public async Task<ActionResult> DeleteProductPhoto(int productId, int productPhotoId)
        {
            var product = await _unitOfWork.ProductRepository.GetProductByIdAsync(productId);

            var productPhoto = product.ProductPhotos.FirstOrDefault(x => x.Id == productPhotoId);

            if (productPhoto == null) return NotFound();

            if (productPhoto.IsMain) return BadRequest("Can not delete main photo.");

            var photo = await _unitOfWork.PhotoRepository.FindPhotoByIdAsync(productPhoto.PhotoId);

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Error != null) return BadRequest(result.Error.Message);

                product.ProductPhotos.Remove(productPhoto);

                _unitOfWork.PhotoRepository.Delete(photo);
            }

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("An error occurred while deleting the image.");

        }

        #endregion

        #region product option API
        [HttpPost("add-option")]
        public async Task<ActionResult<CustomerProductDto>> AddProductOption(Option option)
        {
            option.CreatedByUserId = User.GetUserId();
            option.DateCreated = DateTime.UtcNow;

            _unitOfWork.ProductRepository.Add(option);
            _unitOfWork.ProductRepository.Add(new Stock() {
                Option = option,
                Quantity = 0
            });

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while adding the product option.");
        }

        #endregion

        #region product color API
        [HttpPost("add-color")]
        public async Task<ActionResult<CustomerProductDto>> AddProductColor(Color color)
        {
            _unitOfWork.ProductRepository.Add(color);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while adding the product color.");
        }

        #endregion

        #region product size API
        [HttpPost("add-size")]
        public async Task<ActionResult<CustomerProductDto>> AddProductSize(Size size)
        {
            _unitOfWork.ProductRepository.Add(size);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while adding the product size.");
        }

        #endregion
    }
}