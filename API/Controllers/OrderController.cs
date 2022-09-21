using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Order;
using API.DTOs.Request.OrderRequest;
using API.Entities;
using API.Entities.OrderModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class OrderController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPost("place-order")]
        public async Task<ActionResult> CustomerOrder(OrderRequest orderRequest)
        {
            if (orderRequest.IsFromCart)
            {
                var cartItems = await _unitOfWork.UserRepository.
                                        GetCartItemsByUserIdAndOptions(User.GetUserId(),
                                                orderRequest.OrderItemRequests.Select(x => x.OptionId));
                _unitOfWork.UserRepository.BulkRemove(cartItems);
            }
            var now = DateTime.UtcNow;
            var order = new Order()
            {
                CreatedByUserId = User.GetUserId(),
                DateCreated = now,
                OrderDetails = _mapper.Map<List<OrderDetail>>(orderRequest.OrderItemRequests),
                CurrentStatus = OrderStatus.AwaitingPayment,
                OrderHistories = new List<OrderHistory>() {
                    new OrderHistory() {
                        CreatedByUserId = User.GetUserId(),
                        DateCreated = now,
                        Status = OrderStatus.AwaitingPayment,
                        Note = $"Ordered by customer: {User.GetUsername()} at {now};"
                    }
                }
            };

            _unitOfWork.OrderRepository.Create(order);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Can not create order.");
        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPost("paying")]
        public async Task<ActionResult> CustomerPayingOrder(PayRequest payRequest)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(payRequest.OrderId);

            if (order.CreatedByUserId != User.GetUserId())
                return BadRequest("This is not your order.");

            if (payRequest.PaymentMethod == PaymentMethod.CreditCard || payRequest.PaymentMethod == PaymentMethod.DebitCard)
            {
                #region demo billing process

                Console.WriteLine("Checking card information...");
                Console.WriteLine("Billing...");
                Task.Delay(2000).Wait();
                Random r = new Random();
                int number = r.Next(9999);
                if (number / 10 == 0)
                {
                    Console.WriteLine("Billing failed!");
                    return BadRequest("Can not process this order.");
                }
                Console.WriteLine("Billing success!");

                #endregion

                _unitOfWork.OrderRepository.Create(new OrderHistory
                {
                    OrderId = order.Id,
                    CreatedByUserId = User.GetUserId(),
                    DateCreated = DateTime.UtcNow,
                    Status = OrderStatus.Paid,
                    Note = $"Paid by customer: {User.GetUsername()} at {DateTime.UtcNow}; Payment method: {nameof(payRequest.PaymentMethod)}"
                });

                order.CurrentStatus = OrderStatus.Paid;
                order.LastUpdated = DateTime.UtcNow;
                order.LastUpdatedByUserId = User.GetUserId();

                _unitOfWork.OrderRepository.Update(order);

            }

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Can not process this order.");
        }

        [Authorize(Policy = "BusinessOnly")]
        [HttpPut("processing")]
        public async Task<ActionResult> MoveToProcessing(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);


            _unitOfWork.OrderRepository.Create(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = User.GetUserId(),
                DateCreated = DateTime.UtcNow,
                Status = OrderStatus.Processing,
                Note = $"Create shipping by staff: {User.GetUsername()} at {DateTime.UtcNow};"
            });

            order.CurrentStatus = OrderStatus.Paid;
            order.LastUpdated = DateTime.UtcNow;
            order.LastUpdatedByUserId = User.GetUserId();

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Can not process this order.");
        }

        [Authorize(Policy = "BusinessOnly")]
        [HttpPut("shipping")]
        public async Task<ActionResult> ShippingOrder(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(orderId);

            foreach (var orderDetail in order.OrderDetails)
            {
                var stock = await _unitOfWork.ProductRepository.GetByOptionId(orderDetail.OptionId);
                stock.Quantity -= orderDetail.Quantity;
                _unitOfWork.ProductRepository.Update(stock);
            }

            _unitOfWork.OrderRepository.Create(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = User.GetUserId(),
                DateCreated = DateTime.UtcNow,
                Status = OrderStatus.Shipping,
                Note = $"Change to shipping by staff: {User.GetUsername()} at {DateTime.UtcNow};"
            });

            order.CurrentStatus = OrderStatus.Paid;
            order.LastUpdated = DateTime.UtcNow;
            order.LastUpdatedByUserId = User.GetUserId();

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Can not process this order.");
        }

    }
}