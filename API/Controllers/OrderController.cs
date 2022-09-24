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
using API.Helpers;
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

        #region customer
        

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
            var orderDetails = _mapper.Map<List<OrderDetail>>(orderRequest.OrderItemRequests);
            var order = new Order()
            {
                CreatedByUserId = User.GetUserId(),
                DateCreated = DateTime.UtcNow,
                CurrentStatus = OrderStatus.AwaitingPayment,
                OrderDetails = orderDetails,
                OrderHistories = new List<OrderHistory>() {
                    new OrderHistory() {
                        CreatedByUserId = User.GetUserId(),
                        DateCreated = DateTime.UtcNow,
                        Status = OrderStatus.AwaitingPayment,
                        Note = $"Ordered by customer: {User.GetUsername()} at {DateTime.UtcNow};"
                    }
                }
            };
            _unitOfWork.OrderRepository.Insert(order);

            if (await _unitOfWork.Complete())
            {
                return Ok(order.Id);
            }
            return BadRequest("Can not create order.");
        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPost("paying")]
        public async Task<ActionResult> CustomerPayingOrder(PayRequest payRequest)
        {
            var order = await _unitOfWork.OrderRepository.GetById(payRequest.OrderId);

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

                var paymentMethod = (PaymentMethod) payRequest.PaymentMethod;

                _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
                {
                    OrderId = order.Id,
                    CreatedByUserId = User.GetUserId(),
                    DateCreated = DateTime.UtcNow,
                    Status = OrderStatus.Paid,
                    Note = $"Paid by customer: {User.GetUsername()} at {DateTime.UtcNow}; Payment method: {paymentMethod}"
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

        [Authorize(Policy = "CustomerOnly")]
        [HttpPost("cancel")]
        public async Task<ActionResult> CancellingOrder(CancelOrderRequest cancelOrderRequest)
        {
            var order = await _unitOfWork.OrderRepository.GetById(cancelOrderRequest.OrderId);

            if (order.CurrentStatus == OrderStatus.Shipping)
                return BadRequest("Can not cancel order!");

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = User.GetUserId(),
                DateCreated = DateTime.UtcNow,
                Status = OrderStatus.CancelRequested,
                Note = $"Cancel request created by: {User.GetUsername()} at {DateTime.UtcNow}; Reason: {cancelOrderRequest.Reason}"
            });

            order.CurrentStatus = OrderStatus.CancelRequested;
            order.LastUpdated = DateTime.UtcNow;
            order.LastUpdatedByUserId = User.GetUserId();

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Can not process this order.");
        }


        #endregion

        #region manager


        [Authorize(Policy = "BusinessOnly")]
        [HttpGet("order-list")]
        public async Task<ActionResult> GetOrders([FromQuery] OrderParams orderParams)
        {

            var orders = await _unitOfWork.OrderRepository.GetOrdersAsync(orderParams);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages);

            return Ok(orders);
        }

        [Authorize(Policy = "BusinessOnly")]
        [HttpPut("processing/{orderId}")]
        public async Task<ActionResult> MoveToProcessing(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetById(orderId);


            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = User.GetUserId(),
                DateCreated = DateTime.UtcNow,
                Status = OrderStatus.Processing,
                Note = $"Create shipping by staff: {User.GetUsername()} at {DateTime.UtcNow};"
            });

            order.CurrentStatus = OrderStatus.Processing;
            order.LastUpdated = DateTime.UtcNow;
            order.LastUpdatedByUserId = User.GetUserId();

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Can not process this order.");
        }

        [Authorize(Policy = "BusinessOnly")]
        [HttpPut("shipping/{orderId}")]
        public async Task<ActionResult> ShippingOrder(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetById(orderId);

            foreach (var orderDetail in order.OrderDetails)
            {
                var stock = await _unitOfWork.StockRepository.GetById(orderDetail.OptionId);
                stock.Quantity -= orderDetail.Quantity;
                _unitOfWork.StockRepository.Update(stock);
            }

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = User.GetUserId(),
                DateCreated = DateTime.UtcNow,
                Status = OrderStatus.Shipping,
                Note = $"Change to shipping by staff: {User.GetUsername()} at {DateTime.UtcNow};"
            });

            order.CurrentStatus = OrderStatus.Shipping;
            order.LastUpdated = DateTime.UtcNow;
            order.LastUpdatedByUserId = User.GetUserId();

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Can not process this order.");
        }

        [Authorize(Policy = "BusinessOnly")]
        [HttpPut("shipped/{orderId}")]
        public async Task<ActionResult> VerifyingOrderShipped(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetById(orderId);

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = User.GetUserId(),
                DateCreated = DateTime.UtcNow,
                Status = OrderStatus.Shipped,
                Note = $"Order shipped to customer. Verified by staff: {User.GetUsername()} at {DateTime.UtcNow};"
            });

            order.CurrentStatus = OrderStatus.Shipped;
            order.LastUpdated = DateTime.UtcNow;
            order.LastUpdatedByUserId = User.GetUserId();

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Can not process this order.");
        }
        
        
        #endregion
    }
}