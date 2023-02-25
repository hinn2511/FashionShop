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
    public class FilterController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public FilterController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region anonoymous
        [AllowAnonymous]
        [HttpGet("color")]
        public async Task<ActionResult> GetColorByProductFilter([FromQuery] CustomerProductParams productParams)
        {
            var options = await _unitOfWork.ProductOptionRepository.GetOptionsByProductParams(productParams);
            var groupedOptions = options.GroupBy(g => g.ColorCode)
                                    .Select(g => g.First())
                                    .ToList();
            return Ok(_mapper.Map<List<ColorFilterResponse>>(groupedOptions));
        }
        #endregion

        #region manager

        #endregion


        #region private method

        #endregion
    }
}