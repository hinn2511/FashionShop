using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Params;
using API.DTOs.Request.ConfigurationRequest;
using API.DTOs.Request.ContentRequest;
using API.DTOs.Response;
using API.DTOs.Response.ConfigurationResponse;
using API.DTOs.Response.ContentResponse;
using API.Entities;
using API.Entities.WebPageModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class ConfigurationController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        public ConfigurationController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize(Roles = "ViewCarousels")]
        [HttpGet("carousels")]
        public async Task<ActionResult> GetCarousels([FromQuery] CarouselParams carouselParams)
        {
            var carousels = await _unitOfWork.CarouselRepository.GetCarouselsAsync(carouselParams);

            Response.AddPaginationHeader(carousels.CurrentPage, carousels.PageSize, carousels.TotalCount, carousels.TotalPages);

            var result = _mapper.Map<List<AdminCarouselResponse>>(carousels.ToList());

            return Ok(result);
        }

        [Authorize(Roles = "CreateCarousel")]
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

        [Authorize(Roles = "EditCarousel")]
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

        [Authorize(Roles = "SoftDeleteCarousel")]
        [HttpDelete("soft-delete-carousel")]
        public async Task<ActionResult> SoftDeleteCarousel(DeleteCarouselsRequest deleteCarouselsRequest)
        {
            var carousels = await _unitOfWork.CarouselRepository.GetAllBy(x => deleteCarouselsRequest.Ids.Contains(x.Id));

            if (carousels == null)
                return BadRequest("Carousel not found");

            foreach (var carousel in carousels)
            {
                if (carousel.Status == Status.Deleted)
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

        [Authorize(Roles = "HardDeleteCarousel")]
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

        [Authorize(Roles = "HideCarousels")]
        [HttpPut("hide-or-unhide-carousel")]
        public async Task<ActionResult> HidingCarousel(HideCarouselsRequest hideCarouselsRequest)
        {
            var carousels = await _unitOfWork.CarouselRepository.GetAllBy(x => hideCarouselsRequest.Ids.Contains(x.Id));

            if (carousels == null)
                return BadRequest("Carousel not found");

            foreach (var carousel in carousels)
            {
                if (carousel.Status == Status.Active)
                {
                    carousel.AddHiddenInformation(GetUserId());
                    continue;
                }

                if (carousel.Status == Status.Hidden)
                {
                    carousel.Status = Status.Active;
                    continue;
                }

                if (carousel.Status == Status.Deleted)
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

        [AllowAnonymous]
        [HttpGet("client-settings")]
        public async Task<ActionResult> GetSettings()
        {
            var settings = await _unitOfWork.SettingsRepository.GetFirst();

            if (settings != null)
                return Ok(_mapper.Map<ClientSettingsResponse>(settings));

            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Settings not found."));
        }


        [Authorize(Roles = "UpdateBackgroundSetting")]
        [HttpPut("background/{photoId}")]
        public async Task<ActionResult> UpdateCustomerLoginBackground(int photoId, [FromQuery] string type)
        {
            var settings = await _unitOfWork.SettingsRepository.GetFirst();

            if (settings == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Settings not found."));

            var photo = await _unitOfWork.PhotoRepository.GetFirstBy(x => x.Id == photoId);

            if (photo == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Photo not found."));

            switch (type)
            {
                case "customerLogin":
                    settings.ClientLoginBackground = photo.Url;
                    settings.ClientLoginPhotoId = photo.Id;
                    break;
                case "customerRegister":
                    settings.ClientRegisterBackground = photo.Url;
                    settings.ClientRegisterPhotoId = photo.Id;
                    break;
                default:
                    settings.AdministratorLoginBackground = photo.Url;
                    settings.AdministratorLoginPhotoId = photo.Id;
                    break;
            }           

            _unitOfWork.SettingsRepository.Update(settings);

            if (await _unitOfWork.Complete())
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Settings updated successfully."));

            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Error when updated settings."));
        }

        
    }
}