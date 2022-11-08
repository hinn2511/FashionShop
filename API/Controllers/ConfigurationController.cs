using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.DTOs.Request.ConfigurationRequest;
using API.DTOs.Request.ContentRequest;
using API.DTOs.Response.ConfigurationResponse;
using API.DTOs.Response.ContentResponse;
using API.Entities;
using API.Entities.WebPageModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Policy = "ManagerOnly")]
    public class ConfigurationController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ConfigurationController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("carousels")]
        public async Task<ActionResult> GetCarousels([FromQuery] CarouselParams carouselParams)
        {
            var carousels = await _unitOfWork.CarouselRepository.GetCarouselsAsync(carouselParams);

            Response.AddPaginationHeader(carousels.CurrentPage, carousels.PageSize, carousels.TotalCount, carousels.TotalPages);

            var result = _mapper.Map<List<AdminCarouselResponse>>(carousels.ToList());
            
            return Ok(result);
        }

        [HttpPost("create-carousel")]
        public async Task<ActionResult> AddCarouselImage(AddCarouselRequest addCarouselRequest)
        {
            var carousel = _mapper.Map<Carousel>(addCarouselRequest);

            carousel.AddCreateInformation(GetUserId());
            
            carousel.Status = Status.Hidden;

            _unitOfWork.CarouselRepository.Insert(carousel);

            if (await _unitOfWork.Complete())
                return Ok();
                
            return BadRequest("Can not add new home page.");
        }

        [HttpPut("edit-carousel/{id}")]
        public async Task<ActionResult> UpdateCarousel(int id, UpdateCarouselRequest updateCarouselRequest)
        {
            var carousel = await _unitOfWork.CarouselRepository.GetById(id);

            if (carousel == null)
                return BadRequest("Carousel not found");

            _mapper.Map(updateCarouselRequest, carousel);

            carousel.Id = id;
            carousel.AddUpdateInformation(GetUserId());

            _unitOfWork.CarouselRepository.Update(carousel);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while updating the carousel.");
        }

        [HttpDelete("soft-delete-carousel")]
        public async Task<ActionResult> SoftDeleteCarousel(DeleteCarouselsRequest deleteCarouselsRequest)
        {
            var carousels = await _unitOfWork.CarouselRepository.GetAllBy(x => deleteCarouselsRequest.Ids.Contains(x.Id));

            if (carousels == null)
                return BadRequest("Carousel not found");

            foreach (var carousel in carousels)
            {
                if(carousel.Status == Status.Deleted)
                {
                    continue;
                }
                carousel.AddDeleteInformation(GetUserId());
            }

            _unitOfWork.CarouselRepository.Update(carousels);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while deleting carousels.");
        }

        [HttpDelete("hard-delete-carousel")]
        public async Task<ActionResult> HardDeleteCarousel(DeleteCarouselsRequest deleteCarouselsRequest)
        {
            var carousels = await _unitOfWork.CarouselRepository.GetAllBy(x => deleteCarouselsRequest.Ids.Contains(x.Id));

            if (carousels == null)
                return BadRequest("Carousel not found");

            _unitOfWork.CarouselRepository.Delete(carousels);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while deleting carousels.");
        }

        [HttpPut("hide-or-unhide-carousel")]
        public async Task<ActionResult> HidingCarousel(HideCarouselsRequest hideCarouselsRequest)
        {
            var carousels = await _unitOfWork.CarouselRepository.GetAllBy(x => hideCarouselsRequest.Ids.Contains(x.Id));

            if (carousels == null)
                return BadRequest("Carousel not found");

            foreach (var carousel in carousels)
            {
                if(carousel.Status == Status.Active)
                {
                    carousel.AddHiddenInformation(GetUserId());
                    continue;
                }
                    
                if(carousel.Status == Status.Hidden)
                {
                    carousel.Status = Status.Active;
                    continue;
                }

                if(carousel.Status == Status.Deleted)
                {
                    continue;
                }
            }

            _unitOfWork.CarouselRepository.Update(carousels);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("An error occurred while hiding carousels.");
        }

        
        // [HttpPost("create-home-page")]
        // public async Task<ActionResult> CreateHomePage(HomePageRequest homePageRequest)
        // {
        //     var homePage = _mapper.Map<HomePage>(homePageRequest);

        //     if(await _unitOfWork.HomePageRepository.GetFirstBy(x => x.IsActive) == null)
        //         homePage.IsActive = true;
        //     else
        //         homePage.IsActive = false;
        //     homePage.AddCreateInformation(GetUserId());

        //     foreach (var carousel in homePage.Carousels) {
        //         carousel.AddCreateInformation(GetUserId());
        //     }

        //     foreach (var category in homePage.FeatureCategories) {
        //         category.AddCreateInformation(GetUserId());
        //     }

        //     foreach (var carousel in homePage.FeatureCarousels) {
        //         carousel.AddCreateInformation(GetUserId());
        //     }

        //     _unitOfWork.HomePageRepository.Insert(homePage);

        //     if (await _unitOfWork.Complete())
        //         return Ok();

        //     return BadRequest("Can not add new home page.");
            
        // }

        // [AllowAnonymous]
        // [HttpGet("current-home-page")]
        // public async Task<ActionResult> GetCurrentHomePage()
        // {
        //     var currentActiveHomePage = await _unitOfWork.HomePageRepository.GetFirstBy(x => x.IsActive);
        //     var result = await _unitOfWork.HomePageRepository.GetHomePageByIdAsync(currentActiveHomePage.Id);
        //     return Ok(_mapper.Map<CustomerHomePageResponse>(currentActiveHomePage));
        //     // return Ok(currentActiveHomePage);
        // }

        // [HttpPut("activating-home-page/{id}")]
        // public async Task<ActionResult> ActivatingHomePage(int id)
        // {
        //     var currentActiveHomePage = await _unitOfWork.HomePageRepository.GetFirstBy(x => x.IsActive);

        //     if (currentActiveHomePage.Id == id)
        //         return BadRequest("Home page already activated.");

        //     var newActiveHomePage = await _unitOfWork.HomePageRepository.GetById(id);

        //     if (newActiveHomePage == null)
        //         return BadRequest("Home page not found");

        //     currentActiveHomePage.IsActive = false;

        //     _unitOfWork.HomePageRepository.Update(currentActiveHomePage);
            
        //     newActiveHomePage.IsActive = true;

        //     _unitOfWork.HomePageRepository.Update(newActiveHomePage);

        //     if (await _unitOfWork.Complete())
        //         return Ok();

        //     return BadRequest("Can not activating new home page.");
        // }
    }
}