using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.DTOs.ProductOptionRequest;
using API.DTOs.Request;
using API.DTOs.Response;
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

            foreach(var option in options)
            {
                if (optionResponse.FirstOrDefault(x => x.Color.ColorCode == option.ColorCode) != null)
                    continue;

                var sizes = options.Where(x => x.ColorCode == option.ColorCode).Select(x => new CustomerOptionSizeResponse
                    {
                        SizeName = x.SizeName,
                        OptionId = x.Id,
                        AdditionalPrice = x.AdditionalPrice
                    }
                );
                
                var optionResult = new CustomerOptionResponse
                {
                    Color = new CustomerOptionColorResponse(option.ColorName, option.ColorCode),
                    Sizes = sizes.ToList()
                };
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
        public async Task<ActionResult> GetProductOptionDetailAsAdmin(int id)
        {
            var productOptions = await _unitOfWork.ProductOptionRepository.GetFirstByAndIncludeAsync(x => x.Id == id, "Product", true);
            return Ok(_mapper.Map<AdminOptionDetailResponse>(productOptions));
        }


        [HttpPost("create")]
        public async Task<ActionResult> AddProductOption(CreateProductOptionRequest createProductOptionRequest)
        {
            var option = new Option();
            _mapper.Map(createProductOptionRequest, option);

            if(await _unitOfWork.ProductRepository.GetFirstBy(x => x.Id == createProductOptionRequest.ProductId) == null)
                return BadRequest("Product not found");

            if(await _unitOfWork.ProductOptionRepository.GetFirstBy(x => 
                    x.ProductId == createProductOptionRequest.ProductId 
                    && x.ColorName.ToUpper() == createProductOptionRequest.ColorName.ToUpper()
                    && x.SizeName.ToUpper() == createProductOptionRequest.SizeName.ToUpper()) != null)
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

            var productId = productOption.ProductId;

            _mapper.Map(updateProductOptionRequest, productOption);

            productOption.Id = id;
            productOption.ProductId = productId;
            
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

            foreach (var productOption in productOptions)
            {
                productOption.AddDeleteInformation(User.GetUserId());
            }

            _unitOfWork.ProductOptionRepository.Update(productOptions);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while deleting productOptions.");
        }

        [HttpPut("hide")]
        public async Task<ActionResult> HidingProductOption(BaseBulkRequest hideProductOptionRequest)
        {
            var productOptions = await _unitOfWork.ProductOptionRepository.GetAllBy(x => hideProductOptionRequest.Ids.Contains(x.Id) && x.Status == Status.Active);

            if (productOptions == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Product option not found"));

            foreach (var productOption in productOptions)
            {
                productOption.AddHiddenInformation(GetUserId());

            }

            _unitOfWork.ProductOptionRepository.Update(productOptions);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Successfully hide {productOptions.Count()} product option(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while active product option(s)."));
        }

        [HttpPut("activate")]
        public async Task<ActionResult> ActiveProductOption(BaseBulkRequest activateProductOptionRequest)
        {
            var productOptions = await _unitOfWork.ProductOptionRepository.GetAllBy(x => activateProductOptionRequest.Ids.Contains(x.Id) && x.Status == Status.Hidden);

            if (productOptions == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Product option not found"));

            foreach (var productOption in productOptions)
            {
                productOption.Status = Status.Active;
                productOption.AddUpdateInformation(GetUserId());
            }

            _unitOfWork.ProductOptionRepository.Update(productOptions);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Successfully unhide {productOptions.Count()} product option(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while hiding product option(s)."));
        }

       
        #endregion


        #region private method

        #endregion
    }
}