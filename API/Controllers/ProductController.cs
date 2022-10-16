using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.DTOs.Request.ProductRequest;
using API.DTOs.Response;
using API.DTOs.Response.OptionResponse;
using API.Entities;
using API.Entities.Other;
using API.Entities.OtherModel;
using API.Entities.ProductModel;
using API.Entities.UserModel;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static API.Extensions.StringExtensions;

namespace API.Controllers
{
    [Authorize(Policy = "ManagerOnly")]
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

        #region customer
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetProductsAsCustomer([FromQuery] CustomerProductParams productParams)
        {

            var products = await _unitOfWork.ProductRepository.GetProductsAsync(productParams);

            Response.AddPaginationHeader(products.CurrentPage, products.PageSize, products.TotalCount, products.TotalPages);

            var productsLiked = new List<UserLike>();

            if (User.GetUserId() != 0)
            {
                var userLikes = await _unitOfWork.UserLikeRepository.GetAllBy(x => x.UserId == User.GetUserId());
                productsLiked = userLikes.ToList();
            }

            var result = _mapper.Map<List<CustomerProductsResponse>>(products.ToList());


            if (User.GetUserId() != 0)
            {
                foreach (var item in result)
                {
                    if (productsLiked.Any(x => x.Id == item.Id))
                        item.LikedByUser = true;
                }
            }

            return Ok(result);

        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductAsCustomer(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductDetailWithPhotoAsync(id);
            FilterProductPhoto(product);
            var result = _mapper.Map<CustomerProductDetailResponse>(product);
            if (User.GetUserId() != 0)
            {
                var userLikes = await _unitOfWork.UserLikeRepository.GetAllBy(x => x.UserId == User.GetUserId());
                if (userLikes.Any(x => x.ProductId == product.Id))
                    result.LikedByUser = true;
            }
            return Ok(result);
        }



        [AllowAnonymous]
        [HttpGet("{id}/options")]
        public async Task<ActionResult> GetProductOptionsAsCustomer(int id)
        {
            var options = await _unitOfWork.ProductOptionRepository.GetProductOptionsAsync(id);

            var optionResponse = new List<OptionResponse>();

            foreach (var color in options.Select(x => x.Color).Distinct())
            {
                var sizes = options.Where(x => x.ColorId == color.Id).Select(x => x.Size);
                var optionResult = new OptionResponse
                {
                    Color = _mapper.Map<OptionColorResponse>(color),
                    Sizes = _mapper.Map<List<OptionSizeResponse>>(sizes)
                };
                foreach (var size in optionResult.Sizes)
                {
                    var opt = options.FirstOrDefault(x => x.ColorId == optionResult.Color.Id && x.SizeId == size.Id);
                    size.AdditionalPrice = opt.AdditionalPrice;
                    size.OptionId = opt.Id;
                }
                optionResponse.Add(optionResult);
            }
            return Ok(optionResponse);
        }
        #endregion

        #region manager
        [HttpGet("all")]
        public async Task<ActionResult> GetProductsAsAdmin([FromQuery] AdministratorProductParams productParams)
        {
            var products = await _unitOfWork.ProductRepository.GetProductsAsync(productParams);
            Response.AddPaginationHeader(products.CurrentPage, products.PageSize, products.TotalCount, products.TotalPages);
            var result = _mapper.Map<List<AdminProductsResponse>>(products.ToList());
            return Ok(result);
        }


        [HttpGet("detail/{id}")]
        public async Task<ActionResult> GetProductAsAdmin(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductDetailWithPhotoAsync(id);
            return Ok(_mapper.Map<AdminProductDetailResponse>(product));
        }


        [HttpPost("create")]
        public async Task<ActionResult> AddProduct(CreateProductRequest createProductRequest)
        {
            var product = new Product();
            _mapper.Map(createProductRequest, product);

            product.Slug = product.ProductName.GenerateSlug();
            product.Sold = 0;
            product.AddCreateInformation(User.GetUserId());

            _unitOfWork.ProductRepository.Insert(product);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while adding the product.");
        }


        [HttpPut("edit/{id}")]
        public async Task<ActionResult<CustomerProductDto>> UpdateProduct(int id, UpdateProductRequest updateProductRequest)
        {
            var product = await _unitOfWork.ProductRepository.GetById(id);

            if (product == null)
                return BadRequest("Product not found");

            _mapper.Map(updateProductRequest, product);

            product.Id = id;
            product.Slug = updateProductRequest.ProductName.GenerateSlug();
            product.AddUpdateInformation(User.GetUserId());

            _unitOfWork.ProductRepository.Update(product);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while updating the product.");
        }

        [HttpDelete("soft-delete")]
        public async Task<ActionResult> SoftDeleteProduct(DeleteProductsRequest deleteProductsRequest)
        {
            var products = await _unitOfWork.ProductRepository.GetAllBy(x => deleteProductsRequest.Ids.Contains(x.Id));

            if (products == null)
                return BadRequest("Product not found");

            foreach (var product in products)
            {
                product.AddDeleteInformation(User.GetUserId());
            }

            _unitOfWork.ProductRepository.BulkUpdate(products);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while deleting products.");
        }

        [HttpDelete("hard-delete")]
        public async Task<ActionResult> HardDeleteProduct(DeleteProductsRequest deleteProductsRequest)
        {
            var products = await _unitOfWork.ProductRepository.GetAllBy(x => deleteProductsRequest.Ids.Contains(x.Id));

            if (products == null)
                return BadRequest("Product not found");

            _unitOfWork.ProductRepository.BulkDelete(products.Select(x => x.Id));

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while deleting products.");
        }

        [HttpPut("hide")]
        public async Task<ActionResult> HidingProduct(HideProductsRequest hideProductsRequest)
        {
            var products = await _unitOfWork.ProductRepository.GetAllBy(x => hideProductsRequest.Ids.Contains(x.Id));

            if (products == null)
                return BadRequest("Product not found");

            foreach (var product in products)
            {
                product.AddHiddenInformation(User.GetUserId());
            }

            _unitOfWork.ProductRepository.BulkUpdate(products);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while hiding products.");
        }

        [HttpPut("unhide")]
        public async Task<ActionResult> UndoHidingProduct(HideProductsRequest hideProductsRequest)
        {
            var products = await _unitOfWork.ProductRepository.GetAllBy(x => hideProductsRequest.Ids.Contains(x.Id));

            if (products == null)
                return BadRequest("Product not found");

            foreach (var product in products)
            {
                product.Status = Status.Active;
            }

            _unitOfWork.ProductRepository.BulkUpdate(products);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while undo hiding products.");
        }


        [HttpPost("add-product-photo/{productId}")]
        public async Task<ActionResult> AddProductPhoto(IFormFile file, int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetById(productId);

            if (product == null)
                return BadRequest("Product not found");

            FileExtensions.ValidateFile(file, Constant.ImageContentType);

            var filePath = await FileExtensions.SaveFile(file);

            var resizedFilePath = FileExtensions.ResizeImage(Constant.DefaultImageWidth, Constant.DefaultImageHeight, filePath, true, false);

            var resizedFileName = resizedFilePath.Split("\\").Last();

            var resizedFileExtension = resizedFileName.Split(".").Last();

            var uploadedFile = new UploadedFile()
            {
                DateCreated = DateTime.UtcNow,
                ContentType = file.ContentType,
                Name = resizedFileName,
                Extension = resizedFileExtension,
                Path = resizedFilePath,
                CreatedByUserId = 0
            };

            uploadedFile.AddCreateInformation(User.GetUserId());

            bool IsMain = false;

            if (await _unitOfWork.ProductPhotoRepository.GetFirstBy(x => x.ProductId == productId && x.IsMain) == null)
                IsMain = true;

            var photo = new ProductPhoto
            {
                Url = Constant.DownloadFileUrl + resizedFileName,
                PublicId = "",
                IsMain = IsMain,
                Product = product,
                File = uploadedFile
            };

            photo.AddCreateInformation(User.GetUserId());

            _unitOfWork.ProductPhotoRepository.Insert(photo);

            if (await _unitOfWork.Complete())
            {
                return Ok(_mapper.Map<AdminProductPhotoResponse>(photo));
            }

            return BadRequest("An error occurred while adding the image.");
        }

        [HttpPut("set-main-product-photo/{productId}/{productPhotoId}")]
        public async Task<ActionResult> SetMainProductPhoto(int productId, int productPhotoId)
        {
            var product = await _unitOfWork.ProductRepository.GetById(productId);

            if (product == null)
                return BadRequest("Product not found");

            var newMain = await _unitOfWork.ProductPhotoRepository.GetFirstBy(x => x.Id == productPhotoId);

            if (newMain == null)
                return BadRequest("Product photo not found");

            if (newMain.IsMain)
                return BadRequest("This image is already main image.");

            var currentMain = await _unitOfWork.ProductPhotoRepository.GetFirstBy(x => x.ProductId == productId && x.IsMain);

            if (currentMain != null)
                currentMain.IsMain = false;

            newMain.IsMain = true;

            _unitOfWork.ProductPhotoRepository.Update(currentMain);

            _unitOfWork.ProductPhotoRepository.Update(newMain);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while setting the main image.");
        }

        [HttpDelete("delete-product-photo")]
        public async Task<ActionResult> DeleteProductPhoto(DeleteProductPhotosRequest deleteProductPhotosRequest)
        {
            var productPhotos = await _unitOfWork.ProductPhotoRepository.GetAllBy(x => deleteProductPhotosRequest.Ids.Contains(x.Id));

            if (productPhotos == null)
                return BadRequest("Product photo not found");

            if (productPhotos.Any(x => x.IsMain))
                return BadRequest("Can not delete main photo.");

            var fileIds = productPhotos.Select(x => x.FileId);

            var filesDelete = await _unitOfWork.FileRepository.GetAllBy(x => fileIds.Contains(x .Id));

            foreach(var fileDelete in filesDelete)
                FileExtensions.DeleteFile(fileDelete.Path);

            _unitOfWork.ProductPhotoRepository.BulkDelete(productPhotos.Select(x => x.Id));

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while deleting the image.");

        }

        [HttpPut("hide-product-photo")]
        public async Task<ActionResult> HideProductPhoto(HideProductPhotosRequest hideProductPhotosRequest)
        {
             var productPhotos = await _unitOfWork.ProductPhotoRepository.GetAllBy(x => hideProductPhotosRequest.Ids.Contains(x.Id));

            if (productPhotos == null)
                return BadRequest("Product photo not found");

            if (productPhotos.Any(x => x.IsMain))
                return BadRequest("Can not hide main photo.");

            foreach (var productPhoto in productPhotos)
            {
                productPhoto.AddHiddenInformation(User.GetUserId());
            }

            _unitOfWork.ProductPhotoRepository.BulkUpdate(productPhotos);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while hiding images.");

        }

        [HttpPut("unhide-product-photo")]
        public async Task<ActionResult> UnHideProductPhoto(HideProductPhotosRequest hideProductPhotosRequest)
        {
             var productPhotos = await _unitOfWork.ProductPhotoRepository.GetAllBy(x => hideProductPhotosRequest.Ids.Contains(x.Id));

            if (productPhotos == null)
                return BadRequest("Product photo not found");

            foreach (var productPhoto in productPhotos)
            {
                productPhoto.Status = Status.Active;
            }

            _unitOfWork.ProductPhotoRepository.BulkUpdate(productPhotos);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while un hiding images.");

        }

        #endregion

        #region product option API
        [HttpPost("option")]
        public async Task<ActionResult> AddProductOption(Option option)
        {
            //option.CreatedByUserId = User.GetUserId();
            option.DateCreated = DateTime.UtcNow;

            _unitOfWork.ProductOptionRepository.Insert(option);
            _unitOfWork.StockRepository.Insert(new Stock()
            {
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
        public async Task<ActionResult<CustomerProductDto>> AddProductColor(Entities.ProductModel.Color color)
        {
            _unitOfWork.ColorRepository.Insert(color);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while adding the product color.");
        }

        #endregion

        #region product size API
        [HttpPost("add-size")]
        public async Task<ActionResult<CustomerProductDto>> AddProductSize(Entities.ProductModel.Size size)
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

        private static void FilterProductPhoto(Product product)
        {
            product.ProductPhotos = product.ProductPhotos.Where(x => x.Status == Status.Active).ToList();
        }

        #endregion

        #region comment
        // [HttpPost("add-product-photo/{productId}")]
        // public async Task<ActionResult<ProductPhotoDto>> AddProductPhoto(IFormFile file, int productId)
        // {
        //     var product = await _unitOfWork.ProductRepository.GetById(productId);
        //     var result = await _photoService.AddPhotoAsync(file, 700, 700);

        //     if (result.Error != null) return BadRequest(result.Error.Message);

        //     var photo = new Photo
        //     {
        //         Url = result.SecureUrl.AbsoluteUri,
        //         PublicId = result.PublicId
        //     };

        //     _unitOfWork.PhotoRepository.Insert(photo);

        //     if (await _unitOfWork.Complete())
        //     {
        //         var productPhoto = new ProductPhoto
        //         {
        //             ProductId = productId,
        //             PhotoId = photo.Id,
        //             IsMain = product.ProductPhotos.Count == 0 ? true : false
        //         };

        //         product.ProductPhotos.Add(productPhoto);

        //         if (await _unitOfWork.Complete())
        //         {
        //             return _mapper.Map<ProductPhotoDto>(productPhoto);
        //         }
        //     }

        //     return BadRequest("An error occurred while adding the image.");
        // }

        // [HttpDelete("delete-product-photo/{productId}/{productPhotoId}")]
        // public async Task<ActionResult> DeleteProductPhoto(int productId, int productPhotoId)
        // {
        //     var product = await _unitOfWork.ProductRepository.GetById(productId);

        //     if (product == null)
        //         return BadRequest("Product not found");

        //     var productPhoto = product.ProductPhotos.FirstOrDefault(x => x.Id == productPhotoId);

        //     if (productPhoto == null)
        //         return BadRequest("Product photo not found");

        //     if (productPhoto.IsMain)
        //         return BadRequest("Can not delete main photo.");

        //     var photo = await _unitOfWork.ProductPhotoRepository.GetById(productPhoto.Id);

        //     if (photo.PublicId != null)
        //     {
        //         var result = await _photoService.DeletePhotoAsync(photo.PublicId);

        //         if (result.Error != null) return BadRequest(result.Error.Message);

        //         product.ProductPhotos.Remove(productPhoto);

        //         _unitOfWork.ProductPhotoRepository.Delete(photo);
        //     }

        //     if (await _unitOfWork.Complete()) return Ok();

        //     return BadRequest("An error occurred while deleting the image.");

        // }
        #endregion
    }
}