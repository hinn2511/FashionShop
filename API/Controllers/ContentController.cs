using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs.Response.ContentResponse;
using API.Entities;
using API.Helpers.Authorization;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ContentController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ContentController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        [AllowAnonymous]
        [HttpGet("carousels")]
        public async Task<ActionResult> GetCarousels()
        {
            var carousels = await _unitOfWork.CarouselRepository.GetAllBy(x => x.Status == Status.Active);

            var result = _mapper.Map<IEnumerable<CustomerCarouselResponse>>(carousels);
            
            return Ok(result);
        }
    }
}