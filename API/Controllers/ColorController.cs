using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.Entities;
using API.Entities.ProductModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.Request.ColorRequest;
using API.DTOs.Response.ColorResponse;

namespace API.Controllers
{
    [Authorize(Policy = "ManagerOnly")]
    public class ColorController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ColorController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region anonoymous
        [AllowAnonymous]
        [HttpGet("product-filter")]
        public async Task<ActionResult> GetColorByProductFilter([FromQuery] CustomerProductParams productParams)
        {
            var colors = await _unitOfWork.ColorRepository.GetColorsByProductFilter(productParams);
            return Ok(_mapper.Map<IEnumerable<ColorFilterResponse>>(colors));
        }
        #endregion

        #region manager
        [HttpGet("all")]
        public async Task<ActionResult> GetColorsAsAdmin()
        {
            var colors = await _unitOfWork.ColorRepository.GetAll();
            return Ok(_mapper.Map<IEnumerable<AdminColorResponse>>(colors));
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult> GetColorDetailAsAdmin(int id)
        {
            var color = await _unitOfWork.ColorRepository.GetById(id);
            return Ok(_mapper.Map<AdminColorDetailResponse>(color));
        }


        [HttpPost("create")]
        public async Task<ActionResult> AddColor(CreateColorRequest createColorRequest)
        {
            var color = new Color();

            _mapper.Map(createColorRequest, color);

            //category.Slug = category.ColorName.GenerateSlug();
            color.AddCreateInformation(User.GetUserId());

            _unitOfWork.ColorRepository.Insert(color);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while adding the product colors.");
        }

        [HttpPut("edit/{id}")]
        public async Task<ActionResult> UpdateColor(int id, UpdateColorRequest updateColorRequest)
        {
            var color = await _unitOfWork.ColorRepository.GetById(id);

            _mapper.Map(updateColorRequest, color);

            color.Id = id;
            //color.Slug = updateColorRequest.ColorName.GenerateSlug();
            color.AddUpdateInformation(User.GetUserId());

            _unitOfWork.ColorRepository.Update(color);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while updating the product color.");
        }

        // [HttpDelete("soft-delete")]
        // public async Task<ActionResult> SoftDeleteColor(DeleteColorsRequest deleteColorsRequest)
        // {
        //     var productColors = await _unitOfWork.ColorRepository.GetAllBy(x => deleteColorsRequest.Ids.Contains(x.Id));

        //     if (productColors == null)
        //         return BadRequest("Color not found");

        //     foreach (var category in productColors)
        //     {
        //         category.AddDeleteInformation(User.GetUserId());
        //     }

        //     if (deleteColorsRequest.IncludeProducts)
        //     {
        //         var products = await _unitOfWork.ProductRepository.GetAllBy(x => deleteColorsRequest.Ids.Contains(x.ColorId));
        //         foreach(var product in products)
        //             product.AddDeleteInformation(User.GetUserId());
        //         _unitOfWork.ProductRepository.Update(products);
        //     }

        //     _unitOfWork.ColorRepository.Update(productColors);

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("An error occurred while deleting productColors.");
        // }

        // [HttpDelete("hard-delete")]
        // public async Task<ActionResult> HardDeleteColor(DeleteColorsRequest deleteColorsRequest)
        // {
        //     var productColors = await _unitOfWork.ColorRepository.GetAllBy(x => deleteColorsRequest.Ids.Contains(x.Id));

        //     if (productColors == null)
        //         return BadRequest("Color not found");

        //     _unitOfWork.ColorRepository.Delete(productColors);

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("An error occurred while deleting productColors.");
        // }

        // [HttpPut("hide")]
        // public async Task<ActionResult> HidingColor(HideColorsRequest hideColorsRequest)
        // {
        //     var productColors = await _unitOfWork.ColorRepository.GetAllBy(x => hideColorsRequest.Ids.Contains(x.Id));

        //     if (productColors == null)
        //         return BadRequest("Color not found");

        //     foreach (var category in productColors)
        //     {
        //         category.AddHiddenInformation(User.GetUserId());
        //     }

        //     if (hideColorsRequest.IncludeProducts)
        //     {
        //         var products = await _unitOfWork.ProductRepository.GetAllBy(x => hideColorsRequest.Ids.Contains(x.ColorId));
        //         foreach(var product in products)
        //         {
        //             if(product.Status == Status.Active)
        //                 product.AddHiddenInformation(User.GetUserId());
        //         }
        //         _unitOfWork.ProductRepository.Update(products);
        //     }

        //     _unitOfWork.ColorRepository.Update(productColors);

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("An error occurred while hiding productColors.");
        // }

        // [HttpPut("unhide")]
        // public async Task<ActionResult> UndoHidingColor(HideColorsRequest hideColorsRequest)
        // {
        //     var productColors = await _unitOfWork.ColorRepository.GetAllBy(x => hideColorsRequest.Ids.Contains(x.Id));

        //     if (productColors == null)
        //         return BadRequest("Color not found");

        //     foreach (var category in productColors)
        //     {
        //         category.Status = Status.Active;
        //     }

        //     if (hideColorsRequest.IncludeProducts)
        //     {
        //         var products = await _unitOfWork.ProductRepository.GetAllBy(x => hideColorsRequest.Ids.Contains(x.ColorId));
        //         foreach(var product in products)
        //         {
        //             if(product.Status == Status.Hidden)
        //                 product.Status = Status.Active;
        //         }
        //         _unitOfWork.ProductRepository.Update(products);
        //     }

        //     _unitOfWork.ColorRepository.Update(productColors);

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("An error occurred while undo hiding productColors.");
        // }
        #endregion


        #region private method

        #endregion
    }
}