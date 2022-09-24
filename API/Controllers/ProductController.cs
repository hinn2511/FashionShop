using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.Entities.Other;
using API.Entities.OtherModel;
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
         private static readonly Dictionary<string, string> ContentType = new()
        {
            {"jpg", "image/jpeg" },
            {"jpeg", "image/jpeg" },
            {"png", "image/png" },
            {"mp4", "video/mp4" },
        };

        private readonly string DownloadFileUrl = "https://localhost:5001/file/";

        private static readonly string UploadFolderPath = Path.Combine(Environment.CurrentDirectory, "UploadedFiles");
        private static readonly int DefaultBufferSize = 4096;
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
        public async Task<ActionResult> GetProductAsCustomer(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetById(id);
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

            _unitOfWork.ProductRepository.Insert(product);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while adding the product.");
        }

        [HttpPut("edit")]
        public async Task<ActionResult<CustomerProductDto>> UpdateProduct(UpdateProductDto updateProductDto)
        {
            var product = await _unitOfWork.ProductRepository.GetById(updateProductDto.Id);

            if (product == null)
                return BadRequest("Product not found");

            _mapper.Map(updateProductDto, product);

            product.Slug = updateProductDto.ProductName.GenerateSlug();

            _unitOfWork.ProductRepository.Update(product);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while updating the product.");
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetById(id);

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
            var product = await _unitOfWork.ProductRepository.GetById(productId);
            var result = await _photoService.AddPhotoAsync(file, 700, 700);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            _unitOfWork.PhotoRepository.Insert(photo);

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

        [HttpPost("add-product-photo-local/{productId}")]
        public async Task<ActionResult<ProductPhotoDto>> AddProductPhotoLocal(IFormFile file, int productId)
        {
            ValidateFile(file);

            string name = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                        + "." + file.FileName.Split(".").Last();
            var filePath = Path.Combine(UploadFolderPath, name);

            if (!Directory.Exists(UploadFolderPath))
            {
                Directory.CreateDirectory(UploadFolderPath);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, DefaultBufferSize))
            {
                await file.CopyToAsync(fileStream);
            }

            var uploadedFile = new UploadedFile()
                {
                    DateCreated = DateTime.UtcNow,
                    ContentType = file.ContentType,
                    Name = name,
                    Extension =  file.FileName.Split(".").Last(),
                    Path = filePath,
                    CreatedByUserId = User.GetUserId()
                };

            _unitOfWork.FileRepository.Insert(uploadedFile);

            var product = await _unitOfWork.ProductRepository.GetById(productId);

            await _unitOfWork.Complete();

            var photo = new Photo
            {
                Url = DownloadFileUrl + uploadedFile.Id,
                PublicId = ""
            };

            _unitOfWork.PhotoRepository.Insert(photo);

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
            var product = await _unitOfWork.ProductRepository.GetById(productId);

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
            var product = await _unitOfWork.ProductRepository.GetById(productId);

            var productPhoto = product.ProductPhotos.FirstOrDefault(x => x.Id == productPhotoId);

            if (productPhoto == null) return NotFound();

            if (productPhoto.IsMain) return BadRequest("Can not delete main photo.");

            var photo = await _unitOfWork.PhotoRepository.GetById(productPhoto.PhotoId);

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
        [HttpPost("option")]
        public async Task<ActionResult> AddProductOption(Option option)
        {
            //option.CreatedByUserId = User.GetUserId();
            option.DateCreated = DateTime.UtcNow;

            _unitOfWork.ProductOptionRepository.Insert(option);
            _unitOfWork.StockRepository.Insert(new Stock() {
                Option = option,
                Quantity = 0
            });

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while adding the product option.");
        }

        [HttpGet("{productId}/options")]
        public async Task<ActionResult> GetProductOptions(int productId) 
        {
            return Ok(await _unitOfWork.ProductOptionRepository.GetById(productId));
        }

        #endregion

        #region product color API
        [HttpPost("add-color")]
        public async Task<ActionResult<CustomerProductDto>> AddProductColor(Color color)
        {
            _unitOfWork.ColorRepository.Insert(color);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while adding the product color.");
        }

        #endregion

        #region product size API
        [HttpPost("add-size")]
        public async Task<ActionResult<CustomerProductDto>> AddProductSize(Size size)
        {
            _unitOfWork.SizeRepository.Insert(size);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while adding the product size.");
        }

        #endregion

        #region product brand API
        [HttpPost("add-brand")]
        public async Task<ActionResult<CustomerProductDto>> AddProductSize(Brand brand)
        {
            _unitOfWork.BrandRepository.Insert(brand);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while adding the product brand.");
        }

        #endregion

        #region  private method
         private static void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                throw new ValidationException("File is empty");

            if (!ContentType.Any(ct => ct.Value == file.ContentType))
                throw new ValidationException("Wrong file type");
        }
        #endregion
    }
}