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
using API.DTOs.Params;
using API.DTOs.Request.ProductRequest;
using API.DTOs.Response;
using API.DTOs.Response.OptionResponse;
using API.DTOs.Response.ProductResponse;
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
using Microsoft.VisualBasic.FileIO;
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

            if (GetUserId() != 0)
            {
                var userLikes = await _unitOfWork.UserLikeRepository.GetAllBy(x => x.UserId == GetUserId());
                productsLiked = userLikes.ToList();
            }

            var result = _mapper.Map<List<CustomerProductsResponse>>(products.ToList());


            if (GetUserId() != 0)
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
            if (product.Status != Status.Active)
                return BadRequest("Product not found");

            product.ProductPhotos = FilterHiddenProductPhoto(product);
            
            var result = _mapper.Map<CustomerProductDetailResponse>(product);

            if (GetUserId() != 0)
            {
                var userLikes = await _unitOfWork.UserLikeRepository.GetAllBy(x => x.UserId == GetUserId());
                if (userLikes.Any(x => x.ProductId == product.Id))
                    result.LikedByUser = true;
            }
            return Ok(result);
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

        [HttpPost("import")]
        public async Task<ActionResult> ImportProduct(IFormFile file)
        {

            if (!FileExtensions.ValidateFile(file, Constant.ImportContentType, 0))
                return BadRequest("File not valid");

            var filePath = await FileExtensions.SaveFile(file);

            var importProducts = new List<Product>();

            using (TextFieldParser csvParser = new TextFieldParser(filePath))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                csvParser.ReadLine();
                while (!csvParser.EndOfData)
                {
                    string[] cols = csvParser.ReadFields();

                    int subCategoryId = 0, categoryId = 0, brandId = 0;
                    double price = 0;
                    string name;

                    if (!string.IsNullOrEmpty(cols[0]))
                    {
                        name = cols[0];
                    }
                    else
                        continue;

                    if (!string.IsNullOrEmpty(cols[1]))
                    {

                        bool success = int.TryParse(cols[1], out categoryId);
                        if (!success)
                            continue;
                        if (_unitOfWork.CategoryRepository.GetFirstBy(x => x.Id == categoryId) == null)
                            continue;
                    }
                    else
                        continue;

                    if (!string.IsNullOrEmpty(cols[2]))
                    {

                        bool success = int.TryParse(cols[2], out subCategoryId);
                        if (!success)
                            continue;
                        if (_unitOfWork.SubCategoryRepository.GetFirstBy(x => x.Id == subCategoryId) == null)
                            continue;
                    }

                    if (!string.IsNullOrEmpty(cols[3]))
                    {

                        bool success = int.TryParse(cols[3], out brandId);
                        if (!success)
                            continue;
                        if (_unitOfWork.BrandRepository.GetFirstBy(x => x.Id == brandId) == null)
                            continue;
                    }
                    else
                        continue;

                    if (!string.IsNullOrEmpty(cols[4]))
                    {

                        bool success = double.TryParse(cols[4], out price);
                        if (!success)
                            continue;
                        if (price < 0)
                            continue;
                    }

                    var product = new Product()
                    {
                        ProductName = cols[0],
                        CategoryId = categoryId,
                        SubCategoryId = string.IsNullOrEmpty(cols[2]) ? null : subCategoryId,
                        BrandId = brandId,
                        Price = price,
                    };
                    product.AddCreateInformation(GetUserId());
                    importProducts.Add(product);


                }

            }
            _unitOfWork.ProductRepository.Insert(importProducts);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("An error occurred while importing products.");
        }


        [HttpPost("create")]
        public async Task<ActionResult> AddProduct(CreateProductRequest createProductRequest)
        {
            var product = new Product();
            _mapper.Map(createProductRequest, product);

            product.Slug = product.ProductName.GenerateSlug();
            product.Sold = 0;
            product.AddCreateInformation(GetUserId());

            _unitOfWork.ProductRepository.Insert(product);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while adding the product.");
        }


        [HttpPut("edit/{id}")]
        public async Task<ActionResult> UpdateProduct(int id, UpdateProductRequest updateProductRequest)
        {
            var product = await _unitOfWork.ProductRepository.GetById(id);

            if (product == null)
                return BadRequest("Product not found");

            _mapper.Map(updateProductRequest, product);

            product.Id = id;
            product.Slug = updateProductRequest.ProductName.GenerateSlug();
            product.AddUpdateInformation(GetUserId());

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
                if(product.Status == Status.Deleted)
                {
                    continue;
                }
                product.AddDeleteInformation(GetUserId());
            }

            _unitOfWork.ProductRepository.Update(products);

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

            _unitOfWork.ProductRepository.Delete(products);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while deleting products.");
        }

        [HttpPut("hide-or-unhide")]
        public async Task<ActionResult> HidingProduct(HideProductsRequest hideProductsRequest)
        {
            var products = await _unitOfWork.ProductRepository.GetAllBy(x => hideProductsRequest.Ids.Contains(x.Id));

            if (products == null)
                return BadRequest("Product not found");

            foreach (var product in products)
            {
                if(product.Status == Status.Active)
                {
                    product.AddHiddenInformation(GetUserId());
                    continue;
                }
                    
                if(product.Status == Status.Hidden)
                {
                    product.Status = Status.Active;
                    continue;
                }

                if(product.Status == Status.Deleted)
                {
                    continue;
                }
            }

            _unitOfWork.ProductRepository.Update(products);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while hiding products.");
        }

        [HttpPost("add-product-photo/{productId}")]
        public async Task<ActionResult> AddProductPhoto(IFormFile file, int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetById(productId);

            if (product == null)
                return BadRequest("Product not found");

            if (!FileExtensions.ValidateFile(file, Constant.ImageContentType, 10000))
                return BadRequest("File not valid");

            var filePath = await FileExtensions.SaveFile(file);

            var keepSourceImage = file.ContentType == "image/png";

            var resizedFilePath = FileExtensions.ResizeImage(Constant.DefaultImageWidth, Constant.DefaultImageHeight, filePath, true, true, keepSourceImage);

            var resizedFileName = resizedFilePath.Split("\\").Last();

            var resizedFileExtension = resizedFileName.Split(".").Last();

            var uploadedFile = new UploadedFile()
            {
                ContentType = file.ContentType,
                Name = resizedFileName,
                Extension = resizedFileExtension,
                Path = resizedFilePath
            };
            uploadedFile.AddCreateInformation(GetUserId());

            bool IsMain = false;

            if (await _unitOfWork.ProductPhotoRepository.GetFirstBy(x => x.ProductId == productId && x.IsMain) == null)
                IsMain = true;

            var photo = new ProductPhoto
            {
                Url = Constant.DownloadFileUrl + resizedFileName,
                PublicId = "",
                IsMain = IsMain,
                Product = product,
                File = uploadedFile,
                FileType = Constant.FileType.Image
            };
            photo.AddCreateInformation(GetUserId());

            _unitOfWork.ProductPhotoRepository.Insert(photo);

            if (await _unitOfWork.Complete())
            {
                return Ok(_mapper.Map<AdminProductPhotoResponse>(photo));
            }

            return BadRequest("An error occurred while adding the image.");
        }

        [HttpPost("add-product-video/{productId}")]
        public async Task<ActionResult> AddProductVideo(IFormFile file, int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetById(productId);

            if (product == null)
                return BadRequest("Product not found");

            if (!FileExtensions.ValidateFile(file, Constant.VideoContentType, 20000))
                return BadRequest("File not valid");

            var filePath = await FileExtensions.SaveFile(file);
            var fileName = filePath.Split("\\").Last();
            var fileExtension = filePath.Split(".").Last();

            var uploadedFile = new UploadedFile()
            {
                ContentType = file.ContentType,
                Name = fileName,
                Extension = fileExtension,
                Path = filePath,
            };
            uploadedFile.AddCreateInformation(GetUserId());

            bool IsMain = false;

            if (await _unitOfWork.ProductPhotoRepository.GetFirstBy(x => x.ProductId == productId && x.IsMain) == null)
                IsMain = true;

            var photo = new ProductPhoto
            {
                Url = Constant.DownloadFileUrl + fileName,
                PublicId = "",
                IsMain = IsMain,
                Product = product,
                File = uploadedFile,
                FileType = Constant.FileType.Video
            };

            photo.AddCreateInformation(GetUserId());

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

            var productPhoto = await _unitOfWork.ProductPhotoRepository.GetFirstBy(x => x.Id == productPhotoId);


            if (productPhoto == null)
                return BadRequest("Product photo not found");

            if (productPhoto.IsMain)
                return BadRequest("This image is already main image.");

            productPhoto.IsMain = true;

            productPhoto.AddUpdateInformation(GetUserId());

            _unitOfWork.ProductPhotoRepository.Update(productPhoto);

            var currentMain = await _unitOfWork.ProductPhotoRepository.GetFirstBy(x => x.IsMain && x.ProductId == productId);

            if (currentMain != null)
            {
                currentMain.IsMain = false;

                currentMain.AddUpdateInformation(GetUserId());

                _unitOfWork.ProductPhotoRepository.Update(currentMain);
            }

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

            var filesDelete = await _unitOfWork.FileRepository.GetAllBy(x => productPhotos.Select(x => x.FileId).Contains(x.Id));

            foreach (var fileDelete in filesDelete)
                FileExtensions.DeleteFile(fileDelete.Path);

            _unitOfWork.ProductPhotoRepository.Delete(x => productPhotos.Select(x => x.Id).Contains(x.Id));

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
                productPhoto.AddHiddenInformation(GetUserId());
            }

            _unitOfWork.ProductPhotoRepository.Update(productPhotos);

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

            _unitOfWork.ProductPhotoRepository.Update(productPhotos);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while un hiding images.");

        }

        #endregion

        #region  private method

        private static List<ProductPhoto> FilterHiddenProductPhoto(Product product)
        {
            return product.ProductPhotos.Where(x => x.Status == Status.Active).ToList();
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