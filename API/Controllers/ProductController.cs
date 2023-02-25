using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Params;
using API.DTOs.Request;
using API.DTOs.Request.ProductRequest;
using API.DTOs.Response;
using API.DTOs.Response.ProductResponse;
using API.Entities;
using API.Entities.Other;
using API.Entities.OtherModel;
using API.Entities.ProductModel;
using API.Entities.UserModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
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


            foreach (var item in result)
            {
                if (GetUserId() != 0)
                {
                    if (productsLiked.Any(x => x.ProductId == item.Id))
                        item.LikedByUser = true;
                }
                item.Options = item.Options.GroupBy(g => g.ColorName)
                           .Select(g => g.First()).ToList();
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
        [Authorize(Roles = "ViewProducts")]
        [HttpGet("all")]
        public async Task<ActionResult> GetProductsAsAdmin([FromQuery] AdministratorProductParams productParams)
        {
            var products = await _unitOfWork.ProductRepository.GetProductsAsync(productParams);
            Response.AddPaginationHeader(products.CurrentPage, products.PageSize, products.TotalCount, products.TotalPages);
            var result = _mapper.Map<List<AdminProductsResponse>>(products.ToList());
            return Ok(result);
        }

        [Authorize(Roles = "ViewProductDetail")]
        [HttpGet("detail/{id}")]
        public async Task<ActionResult> GetProductAsAdmin(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductDetailWithPhotoAsync(id);
            return Ok(_mapper.Map<AdminProductDetailResponse>(product));
        }

        [Authorize(Roles = "ImportProduct")]
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

                    int productId = 0, brandId = 0;
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

                        bool success = int.TryParse(cols[1], out productId);
                        if (!success)
                            continue;
                        if (_unitOfWork.ProductRepository.GetFirstBy(x => x.Id == productId) == null)
                            continue;
                    }
                    else
                        continue;

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
                        Slug = cols[0].GenerateSlug(),
                        ProductName = cols[0],
                        CategoryId = productId,
                        BrandId = brandId,
                        Price = price,
                    };
                    product.AddCreateInformation(GetUserId());
                    importProducts.Add(product);


                }

            }
            _unitOfWork.ProductRepository.Insert(importProducts);
            FileExtensions.DeleteFile(filePath);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("An error occurred while importing products.");
        }

        [Authorize(Roles = "CreateProduct")]
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

        [Authorize(Roles = "EditProduct")]
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

        [Authorize(Roles = "SoftDeleteProducts")]
        [HttpDelete("soft-delete")]
        public async Task<ActionResult> SoftDeleteProduct(DeleteProductsRequest deleteProductsRequest)
        {
            var products = await _unitOfWork.ProductRepository.GetAllBy(x => deleteProductsRequest.Ids.Contains(x.Id));

            if (products == null)
                return BadRequest("Product not found");

            foreach (var product in products)
            {
                if (product.Status == Status.Deleted)
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

        [Authorize(Roles = "HardDeleteProducts")]
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

        [Authorize(Roles = "HideProducts")]
        [HttpPut("hide")]
        public async Task<ActionResult> HidingProduct(HideProductsRequest hideProductsRequest)
        {
            var products = await _unitOfWork.ProductRepository.GetAllBy(x => hideProductsRequest.Ids.Contains(x.Id) && x.Status == Status.Active);

            if (products == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Product not found"));

            if (products.Count() == 0)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "No active product found"));

            foreach (var product in products)
            {
                product.AddHiddenInformation(GetUserId());

            }

            _unitOfWork.ProductRepository.Update(products);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Successfully hide {products.Count()} product(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while active product."));
        }

        [Authorize(Roles = "ActivateProducts")]
        [HttpPut("activate")]
        public async Task<ActionResult> ActiveProduct(HideProductsRequest hideProductsRequest)
        {
            var products = await _unitOfWork.ProductRepository.GetAllBy(x => hideProductsRequest.Ids.Contains(x.Id) && x.Status == Status.Hidden);

            if (products == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Product not found"));

            if (products.Count() == 0)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "No hidden product found"));

            foreach (var product in products)
            {
                product.Status = Status.Active;
                product.AddUpdateInformation(GetUserId());
            }

            _unitOfWork.ProductRepository.Update(products);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Successfully unhide {products.Count()} product(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while hiding product."));
        }

        [Authorize(Roles = "DemoteProducts")]
        [HttpPut("demote")]
        public async Task<ActionResult> RemoveEditorChoiceForProduct(BulkDemoteRequest bulkDemoteRequest)
        {
            var products = await _unitOfWork.ProductRepository.GetAllBy(x => bulkDemoteRequest.Ids.Contains(x.Id));

            if (products == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Product not found"));

            if (products.Count() == 0)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "No promoted product found"));

            foreach (var product in products)
            {
                if (product.IsPromoted)
                    product.IsPromoted = false;
            }

            _unitOfWork.ProductRepository.Update(products);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Successfully demote for {products.Count()} product(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while demote for product."));
        }

        [Authorize(Roles = "PromoteProducts")]
        [HttpPut("promote")]
        public async Task<ActionResult> SetEditorChoiceForProduct(BulkPromoteRequest bulkPromoteRequest)
        {
            var products = await _unitOfWork.ProductRepository.GetAllBy(x => bulkPromoteRequest.Ids.Contains(x.Id));

            if (products == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Product not found"));

            if (products.Count() == 0)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "No demoted product found"));

            foreach (var product in products)
            {
                if (!product.IsPromoted)
                    product.IsPromoted = true;
            }

            _unitOfWork.ProductRepository.Update(products);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Successfully promote for {products.Count()} product(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while promote for product."));
        }

        [Authorize(Roles = "CreateProductSale")]
        [HttpPut("create-product-sale")]
        public async Task<ActionResult> CreateProductSale(CreateProductSaleRequest createProductSaleRequest)
        {
            if (createProductSaleRequest.From > createProductSaleRequest.To)
                    return BadRequest(
                        new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Sale date not valid."));
            
            if (createProductSaleRequest.SaleOffType == ProductSaleOffType.None)
                    return BadRequest(
                        new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Sale type not valid."));

            var products = await _unitOfWork.ProductRepository.GetAllBy(x => createProductSaleRequest.Ids.Contains(x.Id));

            if (products == null)
                return BadRequest("Products not found");

            if (createProductSaleRequest.SaleOffType == ProductSaleOffType.SaleOffPercent)
            {
                if (createProductSaleRequest.Percent < 0 || createProductSaleRequest.Percent > 100)
                    return BadRequest(
                        new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Product sale percent not valid. Percent value must between 0 and 100"));
                foreach (var product in products)
                {
                    product.SaleOffPercent = (int)createProductSaleRequest.Percent;
                    product.SaleOffFrom = createProductSaleRequest.From;
                    product.SaleOffTo = createProductSaleRequest.To;
                    product.SaleType = ProductSaleOffType.SaleOffPercent;
                    product.AddUpdateInformation(GetUserId());
                }
            }

            if (createProductSaleRequest.SaleOffType == ProductSaleOffType.SaleOffValue)
            {
                if (products.Any(x => x.Price < createProductSaleRequest.Value))
                    return BadRequest(
                        new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Product sale value not valid. Save value must lower than product price."));
                foreach (var product in products)
                {
                    product.SaleOffValue = (int)createProductSaleRequest.Value;
                    product.SaleOffFrom = createProductSaleRequest.From;
                    product.SaleOffTo = createProductSaleRequest.To;
                    product.SaleType = ProductSaleOffType.SaleOffValue;
                    product.AddUpdateInformation(GetUserId());
                }
            }

            _unitOfWork.ProductRepository.Update(products);

            if (await _unitOfWork.Complete())
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Product sale created successfully."));

            return BadRequest(
                        new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"An error occurred while creating product sale."));

        }

        [Authorize(Roles = "AddProductPhoto")]
        [HttpPost("add-product-photo/{productId}")]
        public async Task<ActionResult> AddProductPhoto(IFormFile file, int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetById(productId);

            if (product == null)
                return BadRequest("Product not found");

            if (!FileExtensions.ValidateFile(file, Constant.ImageContentType, 10000))
                return BadRequest("File not valid");

            var result = await _photoService.AddPhotoAsync(file, 1000, 1000, "lfill", 1);

            if (result.Error != null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "Can not upload photo"));

            bool IsMain = false;

            if (await _unitOfWork.ProductPhotoRepository.GetFirstBy(x => x.ProductId == productId && x.IsMain) == null)
                IsMain = true;

            var photo = new ProductPhoto
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                IsMain = IsMain,
                Product = product,
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

        [Authorize(Roles = "AddProductVideo")]
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

        [Authorize(Roles = "SetProductMainPhoto")]
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
        
        [Authorize(Roles = "DeleteProductPhoto")]
        [HttpDelete("delete-product-photo")]
        public async Task<ActionResult> DeleteProductPhoto(DeleteProductPhotosRequest deleteProductPhotosRequest)
        {
            var productPhotos = await _unitOfWork.ProductPhotoRepository.GetAllBy(x => deleteProductPhotosRequest.Ids.Contains(x.Id));

            if (productPhotos == null)
                return BadRequest("Product photo not found");

            if (productPhotos.Any(x => x.IsMain))
                return BadRequest("Can not delete main photo.");

            _unitOfWork.ProductPhotoRepository.Delete(x => productPhotos.Select(x => x.Id).Contains(x.Id));

            foreach (var photo in productPhotos)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                    return BadRequest(result.Error.Message);
            }

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("An error occurred while deleting the image.");

        }

        [Authorize(Roles = "HideProductPhoto")]
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

        [Authorize(Roles = "ActivateProductPhoto")]
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