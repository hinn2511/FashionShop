using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Request.Cart;
using API.Entities;
using API.Entities.User;
using API.Entities.UserModel;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Policy = "CustomerOnly")]
    public class UserController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        public UserController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _signInManager = signInManager;
        }


        #region user information
        [HttpGet]
        public async Task<ActionResult<CustomerDto>> GetUser()
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId().ToString());
            return _mapper.Map<CustomerDto>(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(CustomerUpdateDto CustomerUpdateDto)
        {
             var user = await _userManager.FindByIdAsync(User.GetUserId().ToString());

            _mapper.Map(CustomerUpdateDto, user);

            await _userManager.UpdateAsync(user);

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to update user!");
        }

        #endregion

        #region user favorites

        [HttpGet("favorite")]
        public async Task<ActionResult> GetUserFavorite(CustomerProductParams customerProductParams)
        {
            var productsLiked = await _unitOfWork.UserLikeRepository.GetUserFavoriteProductsAsync(User.GetUserId(), customerProductParams);
            return Ok(productsLiked);
        }

        [HttpPost("favorite/{productId}")]
        public async Task<ActionResult> AddToFavorite(int productId)
        {
            if (await _unitOfWork.ProductRepository.GetById(productId) == null)
                return BadRequest("Product not found");

            if (await _unitOfWork.UserLikeRepository.GetAllBy(x => x.UserId == User.GetUserId()) != null)
                return BadRequest("The product has been liked by you.");

            _unitOfWork.UserLikeRepository.Insert(new UserLike() {
                ProductId = productId,
                UserId = User.GetUserId(),
                DateCreated = DateTime.UtcNow,
                CreatedByUserId = User.GetUserId()
            });

            if (await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Can not like this product!");
        }

        [HttpDelete("favorite/{productId}")]
        public async Task<ActionResult> RemoveFromFavorite(int productId)
        {
            if (await _unitOfWork.ProductRepository.GetById(productId) == null)
                return BadRequest("Product not found");

            var userLike = await _unitOfWork.UserLikeRepository
                        .GetFirstBy(x => x.UserId == User.GetUserId() && x.ProductId == productId);
            if (userLike == null)
                return BadRequest("The product has not been liked by you.");

            _unitOfWork.UserLikeRepository.Delete(userLike.Id);

            if (await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Can not unlike this product!");
        }

        #endregion
    
        #region user cart 
        [HttpGet("cart")]
        public async Task<ActionResult> GetUserCart()
        {
            return Ok(await _unitOfWork.CartRepository.GetAllBy(x => x.UserId == User.GetUserId()));
        }

        [HttpPost("cart")]
        public async Task<ActionResult> AddToCart(CartRequest cartRequest)
        {
            if (await _unitOfWork.ProductRepository.GetById(cartRequest.OptionId) == null)
                return BadRequest("Product option not found");

            if (cartRequest.Quantity < 1)
                return BadRequest("Quantity not valid.");

            if (await _unitOfWork.CartRepository
                    .GetFirstBy(x => x.UserId == User.GetUserId() 
                        && x.OptionId == cartRequest.OptionId) != null)

                return BadRequest("Can item already exist.");


            _unitOfWork.CartRepository.Insert(new Cart() {
                OptionId = cartRequest.OptionId,
                UserId = User.GetUserId(),
                Quantity = cartRequest.Quantity,
                DateCreated = DateTime.UtcNow,
                CreatedByUserId = User.GetUserId()
            });

            if (await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Can not add new item to cart!");
        }

        [HttpPut("cart")]
        public async Task<ActionResult> UpdateCartItem(UpdateCartRequest updateCartRequest)
        {
            var cartItem = await _unitOfWork.CartRepository
                    .GetFirstBy(x => x.UserId == User.GetUserId() 
                        && x.OptionId == updateCartRequest.OptionId);
            if (cartItem == null)
                return BadRequest("Can item not exist.");

            if (updateCartRequest.Quantity < 1)
                return BadRequest("Quantity not valid.");

            if (await _unitOfWork.ProductOptionRepository.GetById(updateCartRequest.OptionId) == null)
                return BadRequest("Product option not found");

            cartItem.LastUpdated = DateTime.UtcNow;
            cartItem.LastUpdatedByUserId = User.GetUserId();

            cartItem.OptionId = updateCartRequest.OptionId;
            cartItem.Quantity = updateCartRequest.Quantity;

            _unitOfWork.CartRepository.Update(cartItem);

            if (await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Can not add new item to cart!");
        }

        [HttpDelete("cart/{optionId}")]
        public async Task<ActionResult> RemoveFromCart(int optionId)
        {
            var cartItem = await _unitOfWork.CartRepository
                    .GetFirstBy(x => x.UserId == User.GetUserId() 
                        && x.OptionId == optionId);
            if (cartItem == null)
                return BadRequest("Can item not exist.");


            _unitOfWork.CartRepository.Delete(cartItem.Id);

            if (await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Can not unlike this product!");
        }

        #endregion

    }
}