using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.DTOs.ProductOptionRequest;
using API.DTOs.Response.OptionResponse;
using API.Entities;
using API.Entities.ProductModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ProductOptionController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ProductOptionController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region customer
        [AllowAnonymous]
        [HttpGet("{productId}")]
        public async Task<ActionResult> GetProductOptionsAsCustomer(int productId)
        {
            var options = await _unitOfWork.ProductOptionRepository.GetProductOptionsAsCustomerAsync(productId);

            var optionResponse = new List<CustomerOptionResponse>();

            foreach (var color in options.Select(x => x.Color).Distinct())
            {
                var sizes = options.Where(x => x.ColorId == color.Id).Select(x => x.Size);
                var optionResult = new CustomerOptionResponse
                {
                    Color = _mapper.Map<CustomerOptionColorResponse>(color),
                    Sizes = _mapper.Map<List<CustomerOptionSizeResponse>>(sizes)
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
        public async Task<ActionResult> GetProductOptionsAsAdmin([FromQuery] AdminProductOptionParams productOptionParams)
        {
            var productOptions = await _unitOfWork.ProductOptionRepository.GetProductOptionsAsAdminAsync(productOptionParams);
            Response.AddPaginationHeader(productOptions.CurrentPage, productOptions.PageSize, productOptions.TotalCount, productOptions.TotalPages);
            var result = _mapper.Map<List<AdminOptionResponse>>(productOptions.ToList());
            return Ok(result);
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult> GetCategoryDetailAsAdmin(int id)
        {
            var productOptions = await _unitOfWork.ProductOptionRepository.GetFirstByAndIncludeAsync(x => x.Id == id, "Color,Size", true);
            return Ok(_mapper.Map<AdminOptionDetailResponse>(productOptions));
        }


        [HttpPost("create")]
        public async Task<ActionResult> AddProductOption(CreateProductOptionRequest createProductOptionRequest)
        {
            var option = new Option();
            _mapper.Map(createProductOptionRequest, option);

            if(await _unitOfWork.ProductRepository.GetFirstBy(x => x.Id == createProductOptionRequest.ProductId) == null)
                return BadRequest("Product not found");

            if(await _unitOfWork.ColorRepository.GetFirstBy(x => x.Id == createProductOptionRequest.ColorId) == null)
                return BadRequest("Color not found");

            if(await _unitOfWork.SizeRepository.GetFirstBy(x => x.Id == createProductOptionRequest.SizeId) == null)
                return BadRequest("Size not found");

            if(await _unitOfWork.ProductOptionRepository.GetFirstBy(x => 
                    x.ProductId == createProductOptionRequest.ProductId 
                    && x.ColorId == createProductOptionRequest.ColorId 
                    && x.SizeId == createProductOptionRequest.SizeId) != null)
                return BadRequest("Option already exist!");

            option.AddCreateInformation(User.GetUserId());

            _unitOfWork.ProductOptionRepository.Insert(option);

            var defaultStock = new Stock()
            {
                Option = option,
                Quantity = 0
            };

            _unitOfWork.StockRepository.Insert(defaultStock);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while adding the product options.");
        }

        [HttpPut("edit/{id}")]
        public async Task<ActionResult> UpdateProductOption(int id, UpdateProductOptionRequest updateProductOptionRequest)
        {
            var productOption = await _unitOfWork.ProductOptionRepository.GetById(id);

            if (productOption == null)
                return BadRequest("Product option not found");

            if(await _unitOfWork.ColorRepository.GetFirstBy(x => x.Id == updateProductOptionRequest.ColorId) == null)
                return BadRequest("Color not found");

            if(await _unitOfWork.SizeRepository.GetFirstBy(x => x.Id == updateProductOptionRequest.SizeId) == null)
                return BadRequest("Size not found");

            _mapper.Map(updateProductOptionRequest, productOption);

            productOption.Id = id;
            
            productOption.AddUpdateInformation(User.GetUserId());

            _unitOfWork.ProductOptionRepository.Update(productOption);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while updating the product option.");
        }

        [HttpDelete("soft-delete")]
        public async Task<ActionResult> SoftDeleteOption(DeleteProductOptionsRequest deleteProductOptionsRequest)
        {
            var productOptions = await _unitOfWork.ProductOptionRepository.GetAllBy(x => deleteProductOptionsRequest.Ids.Contains(x.Id));

            if (productOptions == null)
                return BadRequest("Option not found");

            foreach (var category in productOptions)
            {
                category.AddDeleteInformation(User.GetUserId());
            }

            _unitOfWork.ProductOptionRepository.Update(productOptions);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while deleting productOptions.");
        }

         [HttpPut("hide-or-unhide")]
        public async Task<ActionResult> HidingCategory(HideProductOptionsRequest hideProductOptionsRequest)
        {
            var productOptions = await _unitOfWork.ProductOptionRepository.GetAllBy(x => hideProductOptionsRequest.Ids.Contains(x.Id));

            if (productOptions == null)
                return BadRequest("Category not found");

            
            foreach (var option in productOptions)
            {
                if(option.Status == Status.Active)
                {
                    option.AddHiddenInformation(GetUserId());
                    continue;
                }
                    
                if(option.Status == Status.Hidden)
                {
                    option.Status = Status.Active;
                    continue;
                }

                if(option.Status == Status.Deleted)
                {
                    continue;
                }
            }

            _unitOfWork.ProductOptionRepository.Update(productOptions);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while hiding productOptions.");
        }

        // [HttpDelete("hard-delete")]
        // public async Task<ActionResult> HardDeleteCategory(DeleteProductOptionsRequest deleteProductOptionsRequest)
        // {
        //     var productOptions = await _unitOfWork.ProductOptionRepository.GetAllBy(x => deleteProductOptionsRequest.Ids.Contains(x.Id));

        //     if (productOptions == null)
        //         return BadRequest("Category not found");

        //     _unitOfWork.ProductOptionRepository.Delete(productOptions);

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("An error occurred while deleting productOptions.");
        // }

       

        // [HttpPut("unhide")]
        // public async Task<ActionResult> UndoHidingCategory(HideProductOptionsRequest hideProductOptionsRequest)
        // {
        //     var productOptions = await _unitOfWork.ProductOptionRepository.GetAllBy(x => hideProductOptionsRequest.Ids.Contains(x.Id));

        //     if (productOptions == null)
        //         return BadRequest("Category not found");

        //     foreach (var category in productOptions)
        //     {
        //         category.Status = Status.Active;
        //     }

        //     if (hideProductOptionsRequest.IncludeProducts)
        //     {
        //         var products = await _unitOfWork.ProductRepository.GetAllBy(x => hideProductOptionsRequest.Ids.Contains(x.CategoryId));
        //         foreach(var product in products)
        //         {
        //             if(product.Status == Status.Hidden)
        //                 product.Status = Status.Active;
        //         }
        //         _unitOfWork.ProductRepository.Update(products);
        //     }

        //     _unitOfWork.ProductOptionRepository.Update(productOptions);

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("An error occurred while undo hiding productOptions.");
        // }
        #endregion


        #region private method

        #endregion
    }
}