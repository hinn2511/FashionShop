using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.DTOs.Response.SizeResponse;
using API.Entities;
using API.Entities.ProductModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.Request.SizeRequest;

namespace API.Controllers
{
    [Authorize(Policy = "ManagerOnly")]
    public class SizeController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SizeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region manager
        [HttpGet("all")]
        public async Task<ActionResult> GetSizesAsAdmin()
        {
            var sizes = await _unitOfWork.SizeRepository.GetAll();
            return Ok(_mapper.Map<IEnumerable<AdminSizeResponse>>(sizes));
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult> GetSizeDetailAsAdmin(int id)
        {
            var size = await _unitOfWork.SizeRepository.GetById(id);
            return Ok(_mapper.Map<AdminSizeDetailResponse>(size));
        }


        [HttpPost("create")]
        public async Task<ActionResult> AddSize(CreateSizeRequest createSizeRequest)
        {
            var size = new Size();

            _mapper.Map(createSizeRequest, size);

            //category.Slug = category.SizeName.GenerateSlug();
            size.AddCreateInformation(User.GetUserId());

            _unitOfWork.SizeRepository.Insert(size);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while adding the product sizes.");
        }

        [HttpPut("edit/{id}")]
        public async Task<ActionResult> UpdateSize(int id, UpdateSizeRequest updateSizeRequest)
        {
            var size = await _unitOfWork.SizeRepository.GetById(id);

            _mapper.Map(updateSizeRequest, size);

            size.Id = id;
            //size.Slug = updateSizeRequest.SizeName.GenerateSlug();
            size.AddUpdateInformation(User.GetUserId());

            _unitOfWork.SizeRepository.Update(size);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while updating the product size.");
        }

        // [HttpDelete("soft-delete")]
        // public async Task<ActionResult> SoftDeleteSize(DeleteSizesRequest deleteSizesRequest)
        // {
        //     var productSizes = await _unitOfWork.SizeRepository.GetAllBy(x => deleteSizesRequest.Ids.Contains(x.Id));

        //     if (productSizes == null)
        //         return BadRequest("Size not found");

        //     foreach (var category in productSizes)
        //     {
        //         category.AddDeleteInformation(User.GetUserId());
        //     }

        //     if (deleteSizesRequest.IncludeProducts)
        //     {
        //         var products = await _unitOfWork.ProductRepository.GetAllBy(x => deleteSizesRequest.Ids.Contains(x.SizeId));
        //         foreach(var product in products)
        //             product.AddDeleteInformation(User.GetUserId());
        //         _unitOfWork.ProductRepository.Update(products);
        //     }

        //     _unitOfWork.SizeRepository.Update(productSizes);

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("An error occurred while deleting productSizes.");
        // }

        // [HttpDelete("hard-delete")]
        // public async Task<ActionResult> HardDeleteSize(DeleteSizesRequest deleteSizesRequest)
        // {
        //     var productSizes = await _unitOfWork.SizeRepository.GetAllBy(x => deleteSizesRequest.Ids.Contains(x.Id));

        //     if (productSizes == null)
        //         return BadRequest("Size not found");

        //     _unitOfWork.SizeRepository.Delete(productSizes);

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("An error occurred while deleting productSizes.");
        // }

        // [HttpPut("hide")]
        // public async Task<ActionResult> HidingSize(HideSizesRequest hideSizesRequest)
        // {
        //     var productSizes = await _unitOfWork.SizeRepository.GetAllBy(x => hideSizesRequest.Ids.Contains(x.Id));

        //     if (productSizes == null)
        //         return BadRequest("Size not found");

        //     foreach (var category in productSizes)
        //     {
        //         category.AddHiddenInformation(User.GetUserId());
        //     }

        //     if (hideSizesRequest.IncludeProducts)
        //     {
        //         var products = await _unitOfWork.ProductRepository.GetAllBy(x => hideSizesRequest.Ids.Contains(x.SizeId));
        //         foreach(var product in products)
        //         {
        //             if(product.Status == Status.Active)
        //                 product.AddHiddenInformation(User.GetUserId());
        //         }
        //         _unitOfWork.ProductRepository.Update(products);
        //     }

        //     _unitOfWork.SizeRepository.Update(productSizes);

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("An error occurred while hiding productSizes.");
        // }

        // [HttpPut("unhide")]
        // public async Task<ActionResult> UndoHidingSize(HideSizesRequest hideSizesRequest)
        // {
        //     var productSizes = await _unitOfWork.SizeRepository.GetAllBy(x => hideSizesRequest.Ids.Contains(x.Id));

        //     if (productSizes == null)
        //         return BadRequest("Size not found");

        //     foreach (var category in productSizes)
        //     {
        //         category.Status = Status.Active;
        //     }

        //     if (hideSizesRequest.IncludeProducts)
        //     {
        //         var products = await _unitOfWork.ProductRepository.GetAllBy(x => hideSizesRequest.Ids.Contains(x.SizeId));
        //         foreach(var product in products)
        //         {
        //             if(product.Status == Status.Hidden)
        //                 product.Status = Status.Active;
        //         }
        //         _unitOfWork.ProductRepository.Update(products);
        //     }

        //     _unitOfWork.SizeRepository.Update(productSizes);

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("An error occurred while undo hiding productSizes.");
        // }
        #endregion


        #region private method

        #endregion
    }
}