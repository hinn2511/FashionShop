using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.DTOs.Request.ProductRequest;
using API.DTOs.Response;
using API.DTOs.Response.OptionResponse;
using API.Entities.Other;
using API.Entities.OtherModel;
using API.Entities.ProductModel;
using API.Extensions;
using API.Helpers;
using API.Helpers.Authorization;
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
            { "jpg", "image/jpeg" },
            { "jpeg", "image/jpeg" },
            { "png", "image/png" },
            { "mp4", "video/mp4" },
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

        #region customer

        [HttpGet]
        public async Task<ActionResult> GetProductsAsCustomer([FromQuery] CustomerProductParams productParams)
        {

            var products = await _unitOfWork.ProductRepository.GetProductsAsync(productParams);

            Response.AddPaginationHeader(products.CurrentPage, products.PageSize, products.TotalCount, products.TotalPages);

            var productsLiked = await _unitOfWork.UserLikeRepository.GetAllBy(x => x.UserId == User.GetUserId());

            var result = _mapper.Map<List<CustomerProductsResponse>>(products.ToList());

            foreach(var item in result)
            {
                if (productsLiked.Any(x => x.Id == item.Id))
                    item.LikedByUser = true;
            }

            return Ok(result);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductAsCustomer(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductWithPhotoAsync(id);
            return Ok(_mapper.Map<CustomerProductDetailResponse>(product));
        }

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

        #region admin
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
            var product = await _unitOfWork.ProductRepository.GetProductWithPhotoAsync(id);
            return Ok(_mapper.Map<AdminProductDetailResponse>(product));
        }

        [HttpPost("create")]
        public async Task<ActionResult> AddProduct(CreateProductRequest createProductRequest)
        {
            var product = new Product();
            _mapper.Map(createProductRequest, product);

            product.Slug = product.ProductName.GenerateSlug();
            product.Sold = 0;
            product.DateCreated = DateTime.UtcNow;
            //product.CreatedByUserId = User.GetUserId();

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

        [HttpPost("add-product-photo/{productId}")]
        public async Task<ActionResult> AddProductPhoto(IFormFile file, int productId)
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

            if (file.ContentType != "video/mp4")
                ResizeImage(1000, 1000, filePath);

            var uploadedFile = new UploadedFile()
            {
                DateCreated = DateTime.UtcNow,
                ContentType = file.ContentType,
                Name = name,
                Extension = file.FileName.Split(".").Last(),
                Path = filePath,
                CreatedByUserId = 0
            };


            _unitOfWork.FileRepository.Insert(uploadedFile);

            var product = await _unitOfWork.ProductRepository.GetById(productId);

            if (product == null)
                return BadRequest("Product not found");

            bool IsMain = false;

            if (await _unitOfWork.ProductPhotoRepository.GetFirstBy(x => x.ProductId == productId && x.IsMain) == null) {
                Console.WriteLine("Main");
                IsMain = true;
            }
            else
                Console.WriteLine("Normal");

            var photo = new ProductPhoto
            {
                Url = DownloadFileUrl + name,
                PublicId = "",
                IsMain = IsMain,
                Product = product
            };

            _unitOfWork.ProductPhotoRepository.Insert(photo);

            if (await _unitOfWork.Complete())
            {
                return Ok(_mapper.Map<ProductPhotoDto>(photo));
            }

            DeleteFile(filePath);

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

            var photo = await _unitOfWork.ProductPhotoRepository.GetById(productPhoto.Id);

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Error != null) return BadRequest(result.Error.Message);

                product.ProductPhotos.Remove(productPhoto);

                _unitOfWork.ProductPhotoRepository.Delete(photo);
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
        private static void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                throw new ValidationException("File is empty");

            if (!ContentType.Any(ct => ct.Value == file.ContentType))
                throw new ValidationException("Wrong file type");
        }

        public void ResizeImage(int newWidth, int newHeight, string stPhotoPath)
        {
            Image imgPhoto = Image.FromFile(stPhotoPath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            if (sourceWidth < sourceHeight)
                SwapDimension(newWidth, newHeight, sourceWidth, sourceHeight);

            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float nPercent = 0, nPercentW = 0, nPercentH = 0;

            nPercentW = ((float)newWidth / (float)sourceWidth);
            nPercentH = ((float)newHeight / (float)sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((newWidth -
                          (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((newHeight -
                          (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int) (sourceWidth * nPercent);
            int destHeight = (int) (sourceHeight * nPercent);


            Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
                          PixelFormat.Format32bppArgb);

            bmPhoto.SetResolution(
                            imgPhoto.HorizontalResolution,
                            imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(System.Drawing.Color.Transparent);
            
            grPhoto.InterpolationMode = System.Drawing.Drawing2D.
                                            InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(-- destX, destY, ++ destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            imgPhoto.Dispose();

            var newPath = stPhotoPath.Replace(stPhotoPath.Split(".").Last(), "png");

            bmPhoto.Save(newPath, ImageFormat.Png);
        }

        private static void SwapDimension(int newWidth, int newHeight, int sourceWidth, int sourceHeight)
        {
            int temp = newWidth;
            newWidth = newHeight;
            newHeight = temp;
                
        }

        private static void DeleteFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        
        #endregion
    }
}