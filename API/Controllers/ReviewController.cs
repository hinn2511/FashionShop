using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.DTOs.Order;
using API.DTOs.Params;
using API.DTOs.Request;
using API.DTOs.Request.OrderRequest;
using API.DTOs.Response;
using API.DTOs.Response.OrderResponse;
using API.DTOs.Response.ReviewResponse;
using API.Entities;
using API.Entities.OrderModel;
using API.Entities.UserModel;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{


    public class ReviewController : BaseApiController
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ReviewController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region customer

        [AllowAnonymous]
        [HttpGet("{productId}")]
        public async Task<ActionResult> GetProductReviewAsCustomer(int productId, [FromQuery] CustomerReviewParams customerReviewParams)
        {
            var product = await _unitOfWork.ProductRepository.GetFirstBy(x => x.Id == productId);

            if (product == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Product not found."));

            var reviews = await _unitOfWork.UserReviewRepository.GetProductReviewsAsync(productId, customerReviewParams);

            Response.AddPaginationHeader(reviews.CurrentPage, reviews.PageSize, reviews.TotalCount, reviews.TotalPages);

            var result = _mapper.Map<List<CustomerReviewResponse>>(reviews.ToList());

            return Ok(result);
        }


        [AllowAnonymous]
        [HttpGet("{productId}/summary")]
        public async Task<ActionResult> GetProductReviewSummaryAsCustomer(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetFirstBy(x => x.Id == productId);

            if (product == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Product not found."));

            var options = await _unitOfWork.ProductOptionRepository.GetAllBy(x => x.ProductId == productId);
            var optionIds = options.Select(x => x.Id);

            var userReviews = await _unitOfWork.UserReviewRepository.GetAllBy(x => optionIds.Contains(x.OptionId));

            var averageScore = userReviews.Select(x => x.Score).Average();

            averageScore = Math.Round(averageScore, 1);

            var scoreGroup = userReviews.GroupBy(x => x.Score).Select(group => new ProductReviewScore(group.Key, group.Count()));

            var scoreList =  scoreGroup.ToList();

            for(int i = 1; i <= 5; i++)
            {
                if(scoreGroup.FirstOrDefault(x => x.Score == i) == null)
                    scoreList.Add(new ProductReviewScore(i, 0));
            }

            var result = new CustomerProductReviewSummary(averageScore, userReviews.Count(), scoreList.OrderByDescending(x => x.Score).ToList());

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{externalId}/reviewed")]
        public async Task<ActionResult> GetProductReviewedItemAsCustomer(string externalId)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.UserId == GetUserId() && x.ExternalId == externalId);
            if (order == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Order not found."));

            if (order.CurrentStatus != OrderStatus.Finished)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Can not create review for unfinished order."));

            var reviewedItems = await _unitOfWork.UserReviewRepository.GetReviewedItemAsync(order.Id);

            var result = _mapper.Map<List<CustomerReviewedItemResponse>>(reviewedItems);

            return Ok(result);
        }


        [Authorize(Policy = "CustomerOnly")]
        [HttpPost("{externalId}")]
        public async Task<ActionResult> CreateReviewOrder(string externalId, CreateCustomerReviewRequest createCustomerReviewRequest)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.UserId == GetUserId() && x.ExternalId == externalId);
            if (order == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Order not found."));

            if (order.CurrentStatus != OrderStatus.Finished)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Can not create review for unfinished order."));

            var orderDetail = await _unitOfWork.OrderDetailRepository.GetFirstBy(x => x.OrderId == order.Id
                                                    && x.OptionId == createCustomerReviewRequest.OptionId);

            if (orderDetail == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Product option not found in order."));

            if (orderDetail.IsReviewed == true)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Can not create review for this order again."));

            createCustomerReviewRequest.Comment = createCustomerReviewRequest.Comment.StripHTML();

            var userReview = _mapper.Map<UserReview>(createCustomerReviewRequest);

            userReview.UserId = GetUserId();
            userReview.OrderId = order.Id;

            userReview.AddCreateInformation(GetUserId());

            _unitOfWork.UserReviewRepository.Insert(userReview);

            orderDetail.IsReviewed = true;

            orderDetail.AddUpdateInformation(GetUserId());

            _unitOfWork.OrderDetailRepository.Update(orderDetail);

            if (await _unitOfWork.Complete())
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Create order review successfully."));

            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Can not create order review."));
        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPut("{externalId}")]
        public async Task<ActionResult> EditReviewOrder(string externalId, EditCustomerReviewRequest editCustomerReviewRequest)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.UserId == GetUserId() && x.ExternalId == externalId);
            if (order == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Order not found."));

            if (order.CurrentStatus != OrderStatus.Finished)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Can not edit review for unfinished order."));

            var orderDetail = await _unitOfWork.OrderDetailRepository.GetFirstBy(x => x.OrderId == order.Id && x.OptionId == editCustomerReviewRequest.OptionId);

            if (orderDetail == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Product option not found in order."));

            if (orderDetail.IsReviewed == false)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Can not edit review for this order."));

            var userReview = await _unitOfWork.UserReviewRepository.GetFirstBy(x => x.UserId == GetUserId()
                                        && x.OptionId == editCustomerReviewRequest.OptionId
                                        && x.OrderId == order.Id);
            if (userReview == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, $"Order review not found."));

            editCustomerReviewRequest.Comment = editCustomerReviewRequest.Comment.StripHTML();

            var userReviewUpdated = _mapper.Map(editCustomerReviewRequest, userReview);

            userReviewUpdated.AddUpdateInformation(GetUserId());

            _unitOfWork.UserReviewRepository.Update(userReview);

            if (await _unitOfWork.Complete())
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Edit order review successfully."));

            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Can not edit order review."));
        }

        #endregion


        #region manager



        #endregion

        #region private method
       

        #endregion
    }
}