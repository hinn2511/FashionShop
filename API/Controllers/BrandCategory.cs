using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.ProductModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    
    public class BrandController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public BrandController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<ActionResult> GetBrandAsCustomer()
        {
            return Ok(await _unitOfWork.BrandRepository.GetAll());
        }

    //     [HttpPost]
    //     public async Task<ActionResult<> AddBrand(BrandRequest brandRequest)
    //     {
    //         var brand = _mapper.Map<Brand>(brandRequest);

    //         brand.CreatedByUserId = GetUserId();
    //         brand.DateCreated = DateTime.UtcNow;

    //         _unitOfWork.BrandRepository.Insert(brand);

    //         if (await _unitOfWork.Complete()) 
    //         {
    //             return Ok();
    //         }
    //         return BadRequest("Error when add brand");
    //     }

    //     [Authorize]
    //     [HttpPut]
    //     public async Task<ActionResult<Brand>> UpdateBrand(UpdateBrandDto updateBrandDto)
    //     {
    //         var brand = await _unitOfWork.BrandRepository.GetById(updateBrandDto.Id);

    //         if(brand == null)
    //             return BadRequest("Brand not found");

    //         _mapper.Map(updateBrandDto, brand);


    //         _unitOfWork.BrandRepository.Update(brand);
    //         if (await _unitOfWork.Complete()) 
    //         {
    //             var result = await _unitOfWork.BrandRepository.GetById(brand.Id);
    //             return Ok(result);
    //         }
    //         return BadRequest("Error when update brand");
    //     }

    //     [HttpDelete("{id}")]
    //     public async Task<ActionResult> DeleteBrand(int id)
    //     {
    //         var brand = await _unitOfWork.BrandRepository.GetById(id);

    //         if(brand == null)
    //             return BadRequest("Brand not found"); 

    //         _unitOfWork.BrandRepository.Delete(brand);
    //         if (await _unitOfWork.Complete()) 
    //         {
    //             return Ok();
    //         }
    //         return BadRequest("Error when delete brand");
    //     }
    
    //     #endregion


    //     #region sub brand
        
    //     [HttpGet("{id}/subCategories")]
    //     public async Task<ActionResult> GetSubCategoriesAsCustomer(int id)
    //     {
    //         return Ok(await _unitOfWork.SubBrandRepository.GetAllBy(x => x.BrandId == id));
    //     }


    //     [HttpPost("subBrand")]
    //     public async Task<ActionResult<CustomerBrandDto>> AddSubBrand(SubBrandRequest subBrandRequest)
    //     {

    //         if (await _unitOfWork.BrandRepository.GetById(subBrandRequest.ParentBrandId) == null)
    //             return BadRequest("Parent brand not found");

    //         var subBrand = _mapper.Map<SubBrand>(subBrandRequest);

    //         subBrand.CreatedByUserId = GetUserId();
    //         subBrand.DateCreated = DateTime.UtcNow;

    //         _unitOfWork.SubBrandRepository.Insert(subBrand);

    //         if (await _unitOfWork.Complete()) 
    //         {
    //             return Ok();
    //         }
    //         return BadRequest("Error when add sub brand");
    //     }

    //     [HttpPut("subBrand")]
    //     public async Task<ActionResult<Brand>> UpdateSubBrand(UpdateSubBrandRequest updateSubBrandRequest)
    //     {
    //         if (await _unitOfWork.SubBrandRepository.GetById(updateSubBrandRequest.ParentBrandId) == null)
    //             return BadRequest("Parent brand not found");

    //         var subBrand = await _unitOfWork.SubBrandRepository.GetById(updateSubBrandRequest.Id);

    //         if(subBrand == null)
    //             return BadRequest("Sub brand not found");


    //         _unitOfWork.SubBrandRepository.Update(_mapper.Map<SubBrand>(updateSubBrandRequest));

    //         if (await _unitOfWork.Complete()) 
    //         {
    //             return Ok();
    //         }
    //         return BadRequest("Error when update sub brand");
    //     }

    //     [HttpDelete("subBrand/{subBrandId}")]
    //     public async Task<ActionResult> DeleteSubBrandBrand(int subBrandId)
    //     {
    //          var subBrand = await _unitOfWork.SubBrandRepository.GetById(subBrandId);

    //         if(subBrand == null)
    //             return BadRequest("Sub brand not found");

    //         _unitOfWork.SubBrandRepository.Delete(subBrand);

    //         if (await _unitOfWork.Complete()) 
    //         {
    //             return Ok();
    //         }

    //         return BadRequest("Error when delete sub brand");
    //     }
    

    //     #endregion
    }
}