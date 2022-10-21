using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Order;
using API.DTOs.Params;
using API.DTOs.Request;
using API.DTOs.Request.OrderRequest;
using API.DTOs.Response.OrderResponse;
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
        [HttpGet]
        public async Task<ActionResult> GetCustomerOrders([FromQuery] CustomerOrderParams customerOrderParams)
        {
            var orders = await _unitOfWork.OrderRepository.GetOrdersAsCustomerAsync(customerOrderParams, GetUserId());

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages);

            var result = _mapper.Map<List<CustomerOrderResponse>>(orders.ToList());

            return Ok(result);

        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpGet("{orderId}/detail")]
        public async Task<ActionResult> GetCustomerOrderDetail(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderWithDetailByIdAsync(orderId);

            if (order.CreatedByUserId != GetUserId())
                return BadRequest("Can not view this order.");

            var result = _mapper.Map<CustomerOrderDetailResponse>(order);

            return Ok(result);

        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPost("place-order")]
        public async Task<ActionResult> PlaceOrder(OrderRequest orderRequest)
        {
            if (orderRequest.IsFromCart)
            {
                var cartItems = await _unitOfWork.CartRepository.
                                        GetAllBy(x => x.UserId == GetUserId() &&
                                                orderRequest.OrderItemRequests.Select(x => x.OptionId).Contains(x.OptionId));
                _unitOfWork.CartRepository.Delete(cartItems);
            }

            var orderDetails = _mapper.Map<List<OrderDetail>>(orderRequest.OrderItemRequests);

            var orderHistories = new List<OrderHistory>();

            if (orderRequest.PaymentMethod == PaymentMethod.Cash)
            {
                orderHistories.Add(new OrderHistory()
                {
                    CreatedByUserId = GetUserId(),
                    DateCreated = DateTime.UtcNow,
                    OrderStatus = OrderStatus.Created,
                    HistoryDescription = $"Ordered by customer: {User.GetUsername()} at {DateTime.UtcNow}; Payment method: Cash."
                });
                orderHistories.Add(new OrderHistory()
                {
                    CreatedByUserId = GetUserId(),
                    DateCreated = DateTime.UtcNow,
                    OrderStatus = OrderStatus.Checking,
                    HistoryDescription = $"Waiting for checking at {DateTime.UtcNow}."
                });
            }
            else
            {
                orderHistories.Add(new OrderHistory()
                {
                    CreatedByUserId = GetUserId(),
                    DateCreated = DateTime.UtcNow,
                    OrderStatus = OrderStatus.Created,
                    HistoryDescription = $"Ordered by customer: {User.GetUsername()} at {DateTime.UtcNow}; Payment method: Credit or Debit card."
                });

                orderHistories.Add(new OrderHistory()
                {
                    CreatedByUserId = GetUserId(),
                    DateCreated = DateTime.UtcNow,
                    OrderStatus = OrderStatus.AwaitingPayment,
                    HistoryDescription = $"Waiting for user payment: {User.GetUsername()} at {DateTime.UtcNow}; Payment method: Credit or Debit card."
                });
            }

            var order = new Order()
            {
                ExternalId = Guid.NewGuid().ToString("N"),
                CurrentStatus = orderRequest.PaymentMethod == PaymentMethod.Cash ? OrderStatus.Checking : OrderStatus.AwaitingPayment,
                OrderDetails = orderDetails,
                OrderHistories = orderHistories,
                PaymentMethod = orderRequest.PaymentMethod
            };

            order.AddCreateInformation(GetUserId());

            _unitOfWork.OrderRepository.Insert(order);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Can not create order.");
        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPost("{orderId}/paid")]
        public async Task<ActionResult> CustomerPayingOrder(int orderId, PayOrderRequest payOrderRequest)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.Id == orderId && x.CreatedByUserId == GetUserId());

            if (order == null)
                return BadRequest("Order not found.");

            if (order.PaymentMethod == PaymentMethod.Cash)
                return BadRequest("Payment method not valid.");

            if (order.CurrentStatus != OrderStatus.AwaitingPayment)
                return BadRequest("Order already paid.");


            // Add your billing provider here
            #region demo billing process

            Console.WriteLine("Checking card information...");
            Console.WriteLine("Billing...");
            Task.Delay(2000).Wait();
            Random r = new Random();
            int number = r.Next(10);
            if (number > 8)
            {
                Console.WriteLine("Billing failed!");
                return BadRequest("Can not process this order.");
            }
            #endregion

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow,
                OrderStatus = OrderStatus.Paid,
                HistoryDescription = $"Paid by customer: {User.GetUsername()} at {DateTime.UtcNow}; With card number: **** **** **** {payOrderRequest.CardNumber.Substring(payOrderRequest.CardNumber.Length - 5, 4)}"
            });

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow,
                OrderStatus = OrderStatus.Processing,
                HistoryDescription = $"Paid by customer: {User.GetUsername()} at {DateTime.UtcNow}; With card number: **** **** **** {payOrderRequest.CardNumber.Substring(payOrderRequest.CardNumber.Length - 5, 4)}"
            });

            order.CurrentStatus = OrderStatus.Paid;
            order.AddUpdateInformation(GetUserId());

            _unitOfWork.OrderRepository.Update(order);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Can not process this order.");
        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPost("{orderId}/request-cancel")]
        public async Task<ActionResult> RequestCancelOrder(int orderId, CancelOrderRequest cancelOrderRequest)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.Id == orderId && x.CreatedByUserId == GetUserId());

            if (order == null)
                return BadRequest("Order not found");

            if (order.CurrentStatus >= OrderStatus.Shipping)
                return BadRequest("Can not cancel order!");

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow,
                OrderStatus = OrderStatus.CancelRequested,
                HistoryDescription = $"Cancel request created by: {User.GetUsername()} at {DateTime.UtcNow}; Reason: {cancelOrderRequest.Reason}"
            });

            order.CurrentStatus = OrderStatus.CancelRequested;
            order.AddUpdateInformation(GetUserId());

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Can not make cancel request for this order.");
        }
        #endregion


        #region manager

        [Authorize(Policy = "ManagerOnly")]
        [HttpPost("check")]
        public async Task<ActionResult> CheckingOrder(BaseBulkRequest bulkRequest)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllBy(x => bulkRequest.Ids.Contains(x.Id));

            if (orders == null)
                return BadRequest("Order not found");

            if (orders.Any(x => x.CurrentStatus != OrderStatus.Checking))
                return BadRequest("Can not checking order!");

            foreach (var order in orders)
            {
                _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
                {
                    OrderId = order.Id,
                    CreatedByUserId = GetUserId(),
                    DateCreated = DateTime.UtcNow,
                    OrderStatus = OrderStatus.Processing,
                    HistoryDescription = $"Order checked by: {User.GetUsername()} at {DateTime.UtcNow}"
                });

                order.CurrentStatus = OrderStatus.Processing;
                order.AddUpdateInformation(GetUserId());
            }

            _unitOfWork.OrderRepository.Update(orders);


            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Can not checking orders.");
        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpPost("{orderId}/cancel")]
        public async Task<ActionResult> CancelOrder(int orderId, CancelOrderRequest cancelOrderRequest)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.Id == orderId);

            if (order == null)
                return BadRequest("Order not found");

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow,
                OrderStatus = OrderStatus.Cancelled,
                HistoryDescription = $"Order is canceled by: {User.GetUsername()} at {DateTime.UtcNow}; Reason: {cancelOrderRequest.Reason}"
            });

            order.CurrentStatus = OrderStatus.Cancelled;
            order.AddUpdateInformation(GetUserId());

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Can not checking this order.");
        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpPost("/ship")]
        public async Task<ActionResult> ShippingOrder(BaseBulkRequest bulkRequest)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllBy(x => bulkRequest.Ids.Contains(x.Id));

            if (orders == null)
                return BadRequest("Order not found");

            if (orders.Any(x => x.CurrentStatus != OrderStatus.Checking))
                return BadRequest("Can not checking order!");

            foreach (var order in orders)
            {
                _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
                {
                    OrderId = order.Id,
                    CreatedByUserId = GetUserId(),
                    DateCreated = DateTime.UtcNow,
                    OrderStatus = OrderStatus.Processing,
                    HistoryDescription = $"Order checked by: {User.GetUsername()} at {DateTime.UtcNow}"
                });

                order.CurrentStatus = OrderStatus.Processing;
                order.AddUpdateInformation(GetUserId());
            }

            _unitOfWork.OrderRepository.Update(orders);


            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            

            return BadRequest("Can not checking this order.");
        }

        #endregion




        // #endregion

        // #region manager


        // [Authorize(Policy = "BusinessOnly")]
        // [HttpGet("order-list")]
        // public async Task<ActionResult> GetOrders([FromQuery] OrderParams orderParams)
        // {

        //     var orders = await _unitOfWork.OrderRepository.GetOrdersAsync(orderParams);

        //     Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages);

        //     return Ok(orders);
        // }

        // [Authorize(Policy = "BusinessOnly")]
        // [HttpPut("processing/{orderId}")]
        // public async Task<ActionResult> MoveToProcessing(int orderId)
        // {
        //     var order = await _unitOfWork.OrderRepository.GetById(orderId);


        //     _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
        //     {
        //         OrderId = order.Id,
        //         CreatedByUserId = GetUserId(),
        //         DateCreated = DateTime.UtcNow,
        //         Status = OrderStatus.Processing,
        //         HistoryDescription = $"Create shipping by staff: {User.GetUsername()} at {DateTime.UtcNow};"
        //     });

        //     order.CurrentStatus = OrderStatus.Processing;
        //     order.LastUpdated = DateTime.UtcNow;
        //     order.LastUpdatedByUserId = GetUserId();

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("Can not process this order.");
        // }

        // [Authorize(Policy = "BusinessOnly")]
        // [HttpPut("shipping/{orderId}")]
        // public async Task<ActionResult> ShippingOrder(int orderId)
        // {
        //     var order = await _unitOfWork.OrderRepository.GetById(orderId);

        //     foreach (var orderDetail in order.OrderDetails)
        //     {
        //         var stock = await _unitOfWork.StockRepository.GetById(orderDetail.OptionId);
        //         stock.Quantity -= orderDetail.Quantity;
        //         _unitOfWork.StockRepository.Update(stock);
        //     }

        //     _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
        //     {
        //         OrderId = order.Id,
        //         CreatedByUserId = GetUserId(),
        //         DateCreated = DateTime.UtcNow,
        //         Status = OrderStatus.Shipping,
        //         HistoryDescription = $"Change to shipping by staff: {User.GetUsername()} at {DateTime.UtcNow};"
        //     });

        //     order.CurrentStatus = OrderStatus.Shipping;
        //     order.LastUpdated = DateTime.UtcNow;
        //     order.LastUpdatedByUserId = GetUserId();

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("Can not process this order.");
        // }

        // [Authorize(Policy = "BusinessOnly")]
        // [HttpPut("shipped/{orderId}")]
        // public async Task<ActionResult> VerifyingOrderShipped(int orderId)
        // {
        //     var order = await _unitOfWork.OrderRepository.GetById(orderId);

        //     _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
        //     {
        //         OrderId = order.Id,
        //         CreatedByUserId = GetUserId(),
        //         DateCreated = DateTime.UtcNow,
        //         Status = OrderStatus.Shipped,
        //         HistoryDescription = $"Order shipped to customer. Verified by staff: {User.GetUsername()} at {DateTime.UtcNow};"
        //     });

        //     order.CurrentStatus = OrderStatus.Shipped;
        //     order.LastUpdated = DateTime.UtcNow;
        //     order.LastUpdatedByUserId = GetUserId();

        //     if (await _unitOfWork.Complete())
        //     {
        //         return Ok();
        //     }
        //     return BadRequest("Can not process this order.");
        // }


        // #endregion
    }
}