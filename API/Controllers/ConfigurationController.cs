using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Customer;
using API.DTOs.Product;
using API.DTOs.Request.ConfigurationRequest;
using API.Entities.ProductModel;
using API.Entities.WebPageModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
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

            if(await _unitOfWork.ConfigurationRepository.GetActiveHomePage() == null)
                homePage.IsActive = true;
            else
                homePage.IsActive = false;
            homePage.DateCreated = DateTime.UtcNow;
            homePage.CreatedByUserId = User.GetUserId();

            foreach (var carousel in homePage.Carousels) {
                carousel.DateCreated = DateTime.UtcNow;
                carousel.CreatedByUserId = User.GetUserId();
            }

            foreach (var category in homePage.FeatureCategories) {
                category.DateCreated = DateTime.UtcNow;
                category.CreatedByUserId = User.GetUserId();
            }

            foreach (var product in homePage.FeatureProducts) {
                product.DateCreated = DateTime.UtcNow;
                product.CreatedByUserId = User.GetUserId();
            }

            _unitOfWork.ConfigurationRepository.Create(homePage);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("Can not add new home page.");
            
        }

        [HttpGet("current-home-page")]
        public async Task<ActionResult> GetCurrentHomePage()
        {
            return Ok(await _unitOfWork.ConfigurationRepository.GetActiveHomePage());
        }

        [HttpPut("activating-home-page/{id}")]
        public async Task<ActionResult> ActivatingHomePage(int id)
        {
            var currentActiveHomePage = await _unitOfWork.ConfigurationRepository.GetActiveHomePage();

            if (currentActiveHomePage.Id == id)
                return BadRequest("Home page already activated.");

            var newActiveHomePage = await _unitOfWork.ConfigurationRepository.GetHomePageById(id);

            if (newActiveHomePage == null)
                return BadRequest("Home page not found");

            currentActiveHomePage.IsActive = false;

            _unitOfWork.ConfigurationRepository.Update(currentActiveHomePage);
            
            newActiveHomePage.IsActive = true;

            _unitOfWork.ConfigurationRepository.Update(newActiveHomePage);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("Can not activating new home page.");
        }
    }
}