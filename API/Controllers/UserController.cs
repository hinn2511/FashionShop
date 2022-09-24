using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Request.Cart;
using API.Entities;
using API.Entities.User;
using API.Entities.UserModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UserController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region user information
        [HttpGet]
        public async Task<ActionResult<CustomerDto>> GetUser()
        {
            var currentUsername = User.GetUsername();
            return await _unitOfWork.UserRepository.GetUserAsync(currentUsername);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(CustomerUpdateDto CustomerUpdateDto)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            _mapper.Map(CustomerUpdateDto, user);

            _unitOfWork.UserRepository.Update(user);

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to update user!");
        }

        #endregion

        #region user favorites

        [HttpGet("favorite")]
        public async Task<ActionResult> GetUserFavorite()
        {
            return Ok(await _unitOfWork.UserRepository.GetAllUserLikes(User.GetUserId()));
        }

        [HttpPost("favorite/{productId}")]
        public async Task<ActionResult> AddToFavorite(int productId)
        {
            if (await _unitOfWork.ProductRepository.GetById(productId) == null)
                return BadRequest("Product not found");

            if (await _unitOfWork.UserRepository.GetUserLike(User.GetUserId(), productId) != null)
                return BadRequest("The product has been liked by you.");

            _unitOfWork.UserRepository.AddToFavorite(new UserLike() {
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

            var userLike = await _unitOfWork.UserRepository.GetUserLike(User.GetUserId(), productId);
            if (userLike == null)
                return BadRequest("The product has not been liked by you.");

            _unitOfWork.UserRepository.RemoveFromFavorite(userLike);

            if (await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Can not unlike this product!");
        }

        #endregion
    
        #region user cart 
        [HttpGet("cart")]
        public async Task<ActionResult> GetUserCart()
        {
            return Ok(await _unitOfWork.UserRepository.GetCartItemsByUserId(User.GetUserId()));
        }

        [HttpPost("cart")]
        public async Task<ActionResult> AddToCart(CartRequest cartRequest)
        {
            if (await _unitOfWork.ProductRepository.GetById(cartRequest.OptionId) == null)
                return BadRequest("Product option not found");

            if (cartRequest.Quantity < 1)
                return BadRequest("Quantity not valid.");

            if (await _unitOfWork.UserRepository.GetCartItemByUserIdAndOptId(User.GetUserId(), cartRequest.OptionId) != null)
                return BadRequest("Can item already exist.");


            _unitOfWork.UserRepository.AddItemToCart(new Cart() {
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
            var cartItem = await _unitOfWork.UserRepository.GetCartItemByUserIdAndOptId(User.GetUserId(), updateCartRequest.OptionId);
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

            _unitOfWork.UserRepository.UpdateCartItem(cartItem);

            if (await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Can not add new item to cart!");
        }

        [HttpDelete("cart/{optionId}")]
        public async Task<ActionResult> RemoveFromCart(int optionId)
        {
            var cartItem = await _unitOfWork.UserRepository.GetCartItemByUserIdAndOptId(User.GetUserId(), optionId);
            if (cartItem == null)
                return BadRequest("Can item not exist.");

            _unitOfWork.UserRepository.RemoveItemFromCart(cartItem);

            if (await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Can not unlike this product!");
        }

        #endregion

    }
}