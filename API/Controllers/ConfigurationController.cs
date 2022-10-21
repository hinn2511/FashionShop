using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Request.ConfigurationRequest;
using API.DTOs.Response.ConfigurationResponse;
using API.Entities.ProductModel;
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
        
        [HttpPost("create-home-page")]
        public async Task<ActionResult> CreateHomePage(HomePageRequest homePageRequest)
        {
            var homePage = _mapper.Map<HomePage>(homePageRequest);

            if(await _unitOfWork.HomePageRepository.GetFirstBy(x => x.IsActive) == null)
                homePage.IsActive = true;
            else
                homePage.IsActive = false;
            homePage.AddCreateInformation(GetUserId());

            foreach (var carousel in homePage.Carousels) {
                carousel.AddCreateInformation(GetUserId());
            }

            foreach (var category in homePage.FeatureCategories) {
                category.AddCreateInformation(GetUserId());
            }

            foreach (var product in homePage.FeatureProducts) {
                product.AddCreateInformation(GetUserId());
            }

            _unitOfWork.HomePageRepository.Insert(homePage);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("Can not add new home page.");
            
        }

        [AllowAnonymous]
        [HttpGet("current-home-page")]
        public async Task<ActionResult> GetCurrentHomePage()
        {
            var currentActiveHomePage = await _unitOfWork.HomePageRepository.GetFirstBy(x => x.IsActive);
            var result = await _unitOfWork.HomePageRepository.GetHomePageByIdAsync(currentActiveHomePage.Id);
            return Ok(_mapper.Map<CustomerHomePageResponse>(currentActiveHomePage));
            // return Ok(currentActiveHomePage);
        }

        [HttpPut("activating-home-page/{id}")]
        public async Task<ActionResult> ActivatingHomePage(int id)
        {
            var currentActiveHomePage = await _unitOfWork.HomePageRepository.GetFirstBy(x => x.IsActive);

            if (currentActiveHomePage.Id == id)
                return BadRequest("Home page already activated.");

            var newActiveHomePage = await _unitOfWork.HomePageRepository.GetById(id);

            if (newActiveHomePage == null)
                return BadRequest("Home page not found");

            currentActiveHomePage.IsActive = false;

            _unitOfWork.HomePageRepository.Update(currentActiveHomePage);
            
            newActiveHomePage.IsActive = true;

            _unitOfWork.HomePageRepository.Update(newActiveHomePage);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("Can not activating new home page.");
        }
    }
}