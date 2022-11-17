using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Params;
using API.DTOs.Request.AccountRequest;
using API.DTOs.Request.CartRequest;
using API.DTOs.Response.AccountResponse;
using API.DTOs.Response.CartResponse;
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
        public async Task<ActionResult> GetUser()
        {
            var user = await _userManager.FindByIdAsync(GetUserId().ToString());
            return Ok(_mapper.Map<AccountResponse>(user));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(UpdateAccountRequest updateAccountRequest)
        {
            var user = await _userManager.FindByIdAsync(GetUserId().ToString());

            _mapper.Map(updateAccountRequest, user);

            await _userManager.UpdateAsync(user);

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to update user!");
        }

        #endregion

        #region user favorites

        [HttpGet("favorite")]
        public async Task<ActionResult> GetUserFavorite(CustomerProductParams customerProductParams)
        {
            var productsLiked = await _unitOfWork.UserLikeRepository.GetUserFavoriteProductsAsync(GetUserId(), customerProductParams);
            return Ok(productsLiked);
        }

        [HttpPost("favorite/{productId}")]
        public async Task<ActionResult> AddToFavorite(int productId)
        {
            if (await _unitOfWork.ProductRepository.GetById(productId) == null)
                return BadRequest("Product not found");

            if (await _unitOfWork.UserLikeRepository.GetAllBy(x => x.UserId == GetUserId()) != null)
                return BadRequest("The product has been liked by you.");

            var newFavorite = new UserLike()
            {
                ProductId = productId,
                UserId = GetUserId(),
            };
            
            newFavorite.AddCreateInformation(GetUserId());
            _unitOfWork.UserLikeRepository.Insert(newFavorite);

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
                        .GetFirstBy(x => x.UserId == GetUserId() && x.ProductId == productId);
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
            CartResponse result = await GetUserCartDetail();
            return Ok(result);
        }

        private async Task<CartResponse> GetUserCartDetail()
        {
            var userCartItems = await _unitOfWork.CartRepository.GetUserCartItems(GetUserId());
            double totalPrice = 0;
            int totalItem = 0;
            foreach (var cartItem in userCartItems)
            {
                var additionalPrice = cartItem.Option.AdditionalPrice;
                var productPrice = cartItem.Option.Product.Price;
                var totalItemPrice = (productPrice + additionalPrice) * cartItem.Quantity;
                totalItem += cartItem.Quantity;
                totalPrice += totalItemPrice;
            }
            var result = new CartResponse()
            {
                CartItems = _mapper.Map<List<CartItemResponse>>(userCartItems),
                TotalPrice = totalPrice,
                TotalItem = totalItem
            };
            return result;
        }

        [HttpPost("cart")]
        public async Task<ActionResult> AddToCart(CreateCartRequest createCartRequest)
        {
            var productOption = await _unitOfWork.ProductOptionRepository.GetById(createCartRequest.OptionId);
            if (productOption == null || productOption.Status != Status.Active) 
                return BadRequest("Product option not found");

            if (createCartRequest.Quantity < 1)
                return BadRequest("Quantity not valid.");
            var cartItem = await _unitOfWork.CartRepository
                    .GetFirstBy(x => x.UserId == GetUserId()
                        && x.OptionId == createCartRequest.OptionId);

            if (cartItem != null)
            {
                cartItem.Quantity += createCartRequest.Quantity;
                _unitOfWork.CartRepository.Update(cartItem);
            }
            else
            {
                var newCartItem = _mapper.Map<Cart>(createCartRequest);
                newCartItem.UserId = GetUserId();
                newCartItem.AddCreateInformation(GetUserId());

                _unitOfWork.CartRepository.Insert(newCartItem);

            }

            if (await _unitOfWork.Complete())
            {
                var cartUpdated = await GetUserCartDetail();
                return Ok(cartUpdated);
            }
            return BadRequest("Can not add new item to cart!");
        }

        [HttpPut("cart")]
        public async Task<ActionResult> UpdateCartItem(UpdateCartRequest updateCartRequest)
        {
            var cartItem = await _unitOfWork.CartRepository
                    .GetFirstBy(x => x.UserId == GetUserId()
                        && x.Id == updateCartRequest.CartId);

            if (cartItem == null)
                return BadRequest("Cart item not exist.");

            if (updateCartRequest.Quantity < 1)
                return BadRequest("Quantity not valid.");

            cartItem.Quantity = updateCartRequest.Quantity;
            
            cartItem.AddUpdateInformation(GetUserId());

            _unitOfWork.CartRepository.Update(cartItem);

            if (await _unitOfWork.Complete())
            {
                var cartUpdated = await GetUserCartDetail();
                return Ok(cartUpdated);
            }
            return BadRequest("Can not update cart item!");
        }

        [HttpPut("cart-after-login")]
        public async Task<ActionResult> UpdateCartItemAfterLogin(UpdateCartAfterLoginRequest updateCartAfterLoginRequest)
        {
            var cartItems = await _unitOfWork.CartRepository
                    .GetAllBy(x => x.UserId == GetUserId());

            foreach(var item in updateCartAfterLoginRequest.NewCartItems)
            {
                var cartItemExist = cartItems.FirstOrDefault(x => x.OptionId == item.OptionId);
                if(cartItemExist != null)
                {
                    cartItemExist.Quantity = item.Quantity;
                    _unitOfWork.CartRepository.Update(cartItemExist);                    
                }
                else
                {
                    var newCartItem = new Cart
                    {
                        UserId = GetUserId(),
                        OptionId = item.OptionId,
                        Quantity = item.Quantity
                    };
                    newCartItem.AddCreateInformation(GetUserId());
                    _unitOfWork.CartRepository.Insert(newCartItem);
                }
            }
            

            // if (cartItem == null)
            //     return BadRequest("Cart item not exist.");

            // if (updateCartRequest.Quantity < 1)
            //     return BadRequest("Quantity not valid.");

            // cartItem.Quantity = updateCartRequest.Quantity;
            
            // cartItem.AddUpdateInformation(GetUserId());

            // _unitOfWork.CartRepository.Update(cartItem);

            if (await _unitOfWork.Complete())
            {
                var cartUpdated = await GetUserCartDetail();
                return Ok(cartUpdated);
            }
            return BadRequest("Can not update cart item!");
        }

        [HttpDelete("cart/{cartItemId}")]
        public async Task<ActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _unitOfWork.CartRepository
                    .GetFirstBy(x => x.UserId == GetUserId()
                        && x.Id == cartItemId);
            if (cartItem == null)
                return BadRequest("Cart item not exist.");

            _unitOfWork.CartRepository.Delete(cartItemId);

            if (await _unitOfWork.Complete())
            {
                var cartUpdated = await GetUserCartDetail();
                return Ok(cartUpdated);
            }
            return BadRequest("Can not unlike this product!");
        }

        #endregion

    }
}