using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Params;
using API.DTOs.Request.AccountRequest;
using API.DTOs.Request.AdminRequest;
using API.DTOs.Request.CartRequest;
using API.DTOs.Request.UserRequest;
using API.DTOs.Response;
using API.DTOs.Response.AccountResponse;
using API.DTOs.Response.AdminResponse;
using API.DTOs.Response.CartResponse;
using API.DTOs.Response.ProductResponse;
using API.DTOs.Response.UserResponse;
using API.Entities;
using API.Entities.ProductModel;
using API.Entities.User;
using API.Entities.UserModel;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class UserController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IRoleService _roleService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        public UserController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IRoleService roleService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _signInManager = signInManager;
            _roleService = roleService;
        }


        #region client
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetUser()
        {
            var user = await _userManager.FindByIdAsync(GetUserId().ToString());
            return Ok(_mapper.Map<AccountResponse>(user));
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateUser(UpdateAccountRequest updateAccountRequest)
        {
            var user = await _userManager.FindByIdAsync(GetUserId().ToString());

            user.DateOfBirth = updateAccountRequest.DateOfBirth;
            user.Email = updateAccountRequest.Email;
            user.FirstName = updateAccountRequest.FirstName;
            user.LastName = updateAccountRequest.LastName;
            user.PhoneNumber = updateAccountRequest.PhoneNumber;
            user.Gender = updateAccountRequest.Gender;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok();

            return BadRequest("Failed to update user!");
        }

        [Authorize]
        [HttpGet("favorite")]
        public async Task<ActionResult> GetUserFavorite([FromQuery] CustomerProductParams customerProductParams)
        {
            var productsLiked = await _unitOfWork.UserLikeRepository.GetUserFavoriteProductsAsync(GetUserId(), customerProductParams);

            Response.AddPaginationHeader(productsLiked.CurrentPage, productsLiked.PageSize, productsLiked.TotalCount, productsLiked.TotalPages);

            var result = _mapper.Map<List<CustomerProductsResponse>>(productsLiked.ToList());

            foreach (var item in result)
            {
                item.LikedByUser = true;
                item.Options = item.Options.GroupBy(g => g.ColorName)
                           .Select(g => g.First()).ToList();
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("favorite/{productId}")]
        public async Task<ActionResult> AddToFavorite(int productId)
        {
            if (await _unitOfWork.ProductRepository.GetById(productId) == null)
                return BadRequest("Product not found");

            if (await _unitOfWork.UserLikeRepository.GetFirstBy(x => x.UserId == GetUserId() && x.ProductId == productId) != null)
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

        [Authorize]
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

        [Authorize]
        [HttpGet("cart")]
        public async Task<ActionResult> GetUserCart()
        {
            CartResponse result = await GetUserCartDetail();
            return Ok(result);
        }

        [Authorize]
        private async Task<CartResponse> GetUserCartDetail()
        {
            var userCartItems = await _unitOfWork.CartRepository.GetUserCartItems(GetUserId());
            double totalPrice = 0;
            int totalItem = 0;
            foreach (var cartItem in userCartItems)
            {
                var additionalPrice = cartItem.Option.AdditionalPrice;
                var productPrice = cartItem.Option.Product.Price;
                var totalItemPrice = 0d;
                if (SaleExtensions.IsProductOnSale(cartItem.Option.Product))
                    totalItemPrice = SaleExtensions.CalculatePriceAfterSaleOff(cartItem.Option) * cartItem.Quantity;
                else
                    totalItemPrice = (productPrice + additionalPrice) * cartItem.Quantity;
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        [HttpPut("cart-after-login")]
        public async Task<ActionResult> UpdateCartItemAfterLogin(UpdateCartAfterLoginRequest updateCartAfterLoginRequest)
        {
            var cartItems = await _unitOfWork.CartRepository
                    .GetAllBy(x => x.UserId == GetUserId());

            foreach (var item in updateCartAfterLoginRequest.NewCartItems)
            {
                var cartItemExist = cartItems.FirstOrDefault(x => x.OptionId == item.OptionId);
                if (cartItemExist != null)
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

            if (await _unitOfWork.Complete())
            {
                var cartUpdated = await GetUserCartDetail();
                return Ok(cartUpdated);
            }
            return BadRequest("Can not update cart item!");
        }

        [Authorize]
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


        #region manager

        [Authorize(Roles = "ViewUsers")]
        [HttpGet("all")]
        public async Task<ActionResult> GetUsers([FromQuery] UserParams param)
        {
            var usersAndRoles = await _roleService.GetUsersAndRolesAsync(param.PageNumber,
                                                                            param.PageSize,
                                                                            param.OrderBy,
                                                                            param.Field,
                                                                            param.Query,
                                                                            param.RoleId,
                                                                            param.UserStatus);

            Response.AddPaginationHeader(usersAndRoles.CurrentPage, usersAndRoles.PageSize, usersAndRoles.TotalCount, usersAndRoles.TotalPages);

            var result = _mapper.Map<List<UserRoleResponse>>(usersAndRoles.ToList());

            return Ok(result);
        }

        [Authorize(Roles = "ViewUserDetail")]
        [HttpGet("detail/{userId}")]
        public async Task<ActionResult> GetUserDetail(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"User not found."));

            var result = _mapper.Map<UserResponse>(user);

            if (user.RoleId != null)
            {
                var userRole = await _unitOfWork.AppRoleRepository.GetFirstBy(x => x.Id == user.RoleId);
                result.Role = userRole.RoleName;
            }
            else
                result.Role = "None";

            var userOrders = await _unitOfWork.OrderRepository.GetAllBy(x => x.UserId == userId);
            result.TotalOrder = userOrders.Count();
            result.TotalAmount = userOrders.Sum(x => x.SubTotal + x.ShippingFee + x.Tax);

            return Ok(result);
        }

        [Authorize(Roles = "ChangePassword")]
        [HttpPut("change-user-password/{userId}")]
        public async Task<ActionResult> UpdateUserPassword(int userId, UpdateUserPasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"User not found."));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);

            if (result.Succeeded)
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Successfully change user {user.UserName}'s password."));

            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Error when change user {user.UserName}'s password."));
        }


        [Authorize(Roles = "SetRole")]
        [HttpPut("set-role/{roleId}")]
        public async Task<ActionResult> SetUsersRole(int roleId, SetUsersRoleRequest setUsersRoleRequest)
        {
            var role = await _roleService.GetRoleByRoleIdAsync(roleId);
            if (role == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Role not found."));

            int success = 0, skip = 0;
            await setUsersRoleRequest.Ids.ForEachAsync(async id =>
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

                if (user != null)
                {
                    if (await _roleService.ApplyRoleForUserAsync(user, role))
                        success++;
                    else
                        skip++;
                }
            });

            return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Successfully add {success} users to role {role.RoleName}. Skip {skip} users"));
        }


        [Authorize(Roles = "RemoveRole")]
        [HttpPut("remove-role")]
        public async Task<ActionResult> UpdateUserRoles(RemoveUsersRoleRequest removeUsersRoleRequest)
        {
            int success = 0, skip = 0;
            await removeUsersRoleRequest.Ids.ForEachAsync(async id =>
           {
               var user = await _userManager.FindByIdAsync(id.ToString());

               if (user != null)
               {
                   if (user.RoleId != null)
                   {
                       skip++;
                   }
                   else
                   {
                       if (await _roleService.RemoveRoleForUserAsync(user))
                           success++;
                       else
                           skip++;
                   }

               }
           });

            return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Successfully remove role for {success} users. Skip {skip} users"));
        }


        [Authorize(Roles = "ActivateUsers")]
        [HttpPut("activate")]
        public async Task<ActionResult> ActivateUser(ActivateUsersRequest activateUsersRequest)
        {
            int success = 0, skip = 0;
            await activateUsersRequest.Ids.ForEachAsync(async id =>
           {

               var user = await _userManager.FindByIdAsync(id.ToString());
               if (user == null)
               {
                   skip++;
               }
               else

               {
                   user.LockoutEnabled = false;
                   user.LockoutEnd = DateTimeOffset.UtcNow;
                   var result = await _userManager.UpdateAsync(user);

                   if (result.Succeeded)
                       success++;
                   else
                       skip++;
               }

           });
            return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Successfully activate {success} users. Skip {skip} users"));
        }


        [Authorize(Roles = "DeactivateUsers")]
        [HttpPut("deactivate")]
        public async Task<ActionResult> DeactivateUser(DeactivateUsersRequest deactivateUsersRequest)
        {
            int success = 0, skip = 0;
            await deactivateUsersRequest.Ids.ForEachAsync(async id =>
          {

              var user = await _userManager.FindByIdAsync(id.ToString());
              if (user == null || user.Id == GetUserId())
              {
                  skip++;
              }
              else
              {
                  user.LockoutEnabled = true;
                  user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(200);
                  var result = await _userManager.UpdateAsync(user);

                  if (result.Succeeded)
                      success++;
                  else
                      skip++;
              }


          });


            return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Successfully deactivate {success} users. Skip {skip} users"));
        }

        [Authorize(Roles = "DeleteUsers")]
        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteUser(DeleteUsersRequest deleteUsersRequest)
        {
            int success = 0, skip = 0;
            await deleteUsersRequest.Ids.ForEachAsync(async id =>
        {

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || user.Id == GetUserId())
            {
                skip++;

            }
            else
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                    success++;
                else
                    skip++;
            }
        });


            return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Successfully delete {success} users. Skip {skip} users"));
        }


        #endregion


        #region private method      


        #endregion

    }
}