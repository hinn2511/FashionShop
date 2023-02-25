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
            var orders = await _unitOfWork.OrderRepository.GetOrdersAsync(customerOrderParams, GetUserId());

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages);

            var result = _mapper.Map<List<CustomerOrderResponse>>(orders.ToList());

            return Ok(result);

        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpGet("{externalOrderId}")]
        public async Task<ActionResult> GetCustomerOrderDetail(string externalOrderId)
        {
            if (!StringExtensions.IsValidEan13(externalOrderId))
                return NotFound("Order not found");

            var order = await _unitOfWork.OrderRepository.GetOrderWithDetailByExternalIdAsync(externalOrderId);

            if (order.CreatedByUserId != GetUserId())
                return BadRequest("Can not view this order.");

            var result = _mapper.Map<CustomerOrderResponse>(order);

            return Ok(result);

        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPost("place-order")]
        public async Task<ActionResult> PlaceOrder(OrderRequest orderRequest)
        {
            var orderDetails = new List<OrderDetail>();
            if (orderRequest.IsFromCart)
            {
                var cartItems = await _unitOfWork.CartRepository.
                                        GetAllBy(x => x.UserId == GetUserId());
                if (!cartItems.Any())
                    return BadRequest("Your cart is empty.");

                var optionIds = cartItems.Select(x => x.OptionId);
                var options = await _unitOfWork.ProductOptionRepository.GetAllAndIncludeAsync(x => optionIds.Contains(x.Id), "Product", true);

                foreach (var item in cartItems)
                {
                    var option = options.FirstOrDefault(x => x.Id == item.OptionId);
                    var orderDetail = new OrderDetail
                    {
                        OptionId = item.OptionId,
                        Quantity = item.Quantity
                    };
                    if(SaleExtensions.IsProductOnSale(option.Product))
                        orderDetail.Price = SaleExtensions.CalculatePriceAfterSaleOff(option);
                    else
                        orderDetail.Price = option.Product.Price + option.AdditionalPrice;                    
                    orderDetail.Total = orderDetail.Price * orderDetail.Quantity;

                    orderDetail.AddCreateInformation(GetUserId());
                    orderDetails.Add(orderDetail);
                    
                    _unitOfWork.CartRepository.Delete(item.Id);
                }
            }
            else
            {
                orderDetails = _mapper.Map<List<OrderDetail>>(orderRequest.OrderItemRequests);
            }

            var orderHistories = new List<OrderHistory>();

            orderHistories.Add(new OrderHistory()
            {
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.Created,
                HistoryDescription = $"Ordered by customer: {User.GetUsername()} at {DateTime.UtcNow.AddMonths(3)}; Payment method: {orderRequest.PaymentMethod.ConvertToString()}."
            });

            orderHistories.Add(new OrderHistory()
            {
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.Checking,
                HistoryDescription = $"Waiting for verifying at {DateTime.UtcNow.AddMonths(3)}."
            });

            var shippingMethodData = await System.IO.File.ReadAllTextAsync("Data/LocalData/ShippingMethod.json");

            var shippingMethods = JsonSerializer.Deserialize<List<ShippingMethod>>(shippingMethodData);

            var selectedShippingMethod = shippingMethods.FirstOrDefault(x => x.Id == orderRequest.ShippingMethod);

            var now = DateTime.UtcNow.AddMonths(3);

            var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            var endOfDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            var orderIdDate = now.ToString("yyMMdd");

            var orderTodayCount = await _unitOfWork.OrderRepository.GetAllBy(x => x.DateCreated > today && x.DateCreated < endOfDay);
            var orderIdNumber = (orderTodayCount.Count() + 1).ToString().PadLeft(6, '0');

            var order = new Order()
            {
                Address = orderRequest.Address,
                ReceiverName = orderRequest.ReceiverName,
                PhoneNumber = orderRequest.PhoneNumber,
                Email = orderRequest.Email,
                ExternalId = ($"{orderIdDate}{orderIdNumber}").ConvertToEan13(),
                CurrentStatus = OrderStatus.Checking,
                OrderDetails = orderDetails,
                OrderHistories = orderHistories,
                PaymentMethod = orderRequest.PaymentMethod,
                ShippingMethod = selectedShippingMethod.Name,
                ShippingFee = selectedShippingMethod.Fee,
                SubTotal = orderDetails.Any() ? orderDetails.Sum(x => x.Total) : 0,
                Tax = orderDetails.Any() ? orderDetails.Sum(x => x.Total) / 100 * Constant.taxPercent : 0,
                UserId = GetUserId()
            };

            order.AddCreateInformation(GetUserId());

            _unitOfWork.OrderRepository.Insert(order);

            if (await _unitOfWork.Complete())
            {
                return Ok(order.ExternalId);
            }
            return BadRequest("Can not create order.");
        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPost("{externalOrderId}/paid-by-card")]
        public async Task<ActionResult> CustomerPayingOrder(string externalOrderId, PayOrderRequest payOrderRequest)
        {

            if (!StringExtensions.IsValidEan13(externalOrderId))
                return BadRequest("Order ID not valid.");

            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.ExternalId == externalOrderId && x.CreatedByUserId == GetUserId());

            if (order == null)
                return BadRequest("Order not found.");

            if (order.PaymentMethod != PaymentMethod.CreditCard && order.PaymentMethod != PaymentMethod.DebitCard)
                return BadRequest("Payment method not valid.");

            if (order.CurrentStatus != OrderStatus.Created && order.CurrentStatus != OrderStatus.Checking)
                return BadRequest("Order already paid.");

            // Add your billing provider here
            #region demo billing process

            Console.WriteLine("Checking card information...");
            Console.WriteLine("Billing...");
            Task.Delay(3000).Wait();
            Random r = new Random();
            int number = r.Next(10);
            if (number > 8)
            {
                Console.WriteLine("Billing failed!");
                _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
                {
                    OrderId = order.Id,
                    CreatedByUserId = GetUserId(),
                    DateCreated = DateTime.UtcNow.AddMonths(3),
                    OrderStatus = OrderStatus.Checking,
                    HistoryDescription = $"Payment failed by customer: {User.GetUsername()} at {DateTime.UtcNow.AddMonths(3)}; With card number: **** **** **** {payOrderRequest.CardNumber.Substring(payOrderRequest.CardNumber.Length - 4, 4)}."
                });
                return BadRequest("We can not verifying your card information right now. Please try again later.");
            }
            Console.WriteLine("Billing successfully!");
            #endregion

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.Paid,
                HistoryDescription = $"Paid successfully by customer: {User.GetUsername()} at {DateTime.UtcNow.AddMonths(3)}; With card number: **** **** **** {payOrderRequest.CardNumber.Substring(payOrderRequest.CardNumber.Length - 4, 4)}."
            });

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.Processing,
                HistoryDescription = $"Order has been paid. Checking passed and move to processing automatically at {DateTime.UtcNow.AddMonths(3)}."
            });

            order.CurrentStatus = OrderStatus.Processing;

            order.AddUpdateInformation(GetUserId());

            _unitOfWork.OrderRepository.Update(order);

            await UpdateStockQuantity(order.Id, order, true);

            if (await _unitOfWork.Complete())
            {
                return Ok(order.ExternalId);
            }
            return BadRequest("Can not process this order.");
        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPut("{externalId}/cancel-requested")]
        public async Task<ActionResult> RequestCancelOrder(string externalId, CancelOrderRequest cancelOrderRequest)
        {
            if (!StringExtensions.IsValidEan13(externalId))
                return BadRequest("Order ID not valid.");

            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.ExternalId == externalId && x.UserId == GetUserId());

            if (order == null)
                return BadRequest("Order not found");

            var allowCancelRequestStatus = new List<OrderStatus>()
            {
                OrderStatus.Created,
                OrderStatus.Checking,
                OrderStatus.Paid,
                OrderStatus.Processing
            };

            if (!allowCancelRequestStatus.Any(x => x == order.CurrentStatus))
                return BadRequest("Can not request order cancellation!");

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.CancelRequested,
                HistoryDescription = $"Order cancellation requested by: {User.GetUsername()} at {DateTime.UtcNow.AddMonths(3)}.Reason: {cancelOrderRequest.Reason}"
            });

            order.CurrentStatus = OrderStatus.CancelRequested;
            order.AddUpdateInformation(GetUserId());

            _unitOfWork.OrderRepository.Update(order);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Can not verify orders.");
        }

        [Authorize(Policy = "CustomerOnly")]
        [HttpPut("{externalId}/confirm-delivered")]
        public async Task<ActionResult> ConfirmReceiveOrder(string externalId)
        {
            if (!StringExtensions.IsValidEan13(externalId))
                return BadRequest("Order ID not valid.");
                
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.ExternalId == externalId && x.UserId == GetUserId());

            if (order == null)
                return BadRequest("Order not found");


            if (order.CurrentStatus != OrderStatus.Shipped)
                return BadRequest("Can not confirm order delivery!");

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.Finished,
                HistoryDescription = $"Order had been delivered to customer: {User.GetUsername()} at {DateTime.UtcNow.AddMonths(3)}."
            });

            order.CurrentStatus = OrderStatus.Finished;
            order.AddUpdateInformation(GetUserId());

            _unitOfWork.OrderRepository.Update(order);

            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAllAndIncludeAsync(x => x.OrderId == order.Id, "Option", true);

            foreach(var orderDetail in orderDetails)
            {
                var product = await _unitOfWork.ProductRepository.GetFirstBy(x => x.Id == orderDetail.Option.ProductId);
                if (product != null)
                    product.Sold += orderDetail.Quantity;
                _unitOfWork.ProductRepository.Update(product);
            }

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Can not verify orders.");
        }


        [Authorize(Policy = "CustomerOnly")]
        [HttpPut("{externalId}/return-requested")]
        public async Task<ActionResult> RequestReturnOrder(string externalId, ReturnOrderRequest returnOrderRequest)
        {
            if (!StringExtensions.IsValidEan13(externalId))
                return BadRequest("Order ID not valid.");
                
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.ExternalId == externalId && x.UserId == GetUserId());

            if (order == null)
                return BadRequest("Order not found");

            if (order.CurrentStatus != OrderStatus.Shipped)
                return BadRequest("Can not request order return!");

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.ReturnRequested,
                HistoryDescription = $"Order return requested by: {User.GetUsername()} at {DateTime.UtcNow.AddMonths(3)}. Reason: {returnOrderRequest.Reason}"
            });

            order.CurrentStatus = OrderStatus.ReturnRequested;
            order.AddUpdateInformation(GetUserId());

            _unitOfWork.OrderRepository.Update(order);
            
            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Can not return orders.");
        }
        
        #endregion


        #region manager

        [Authorize(Policy = "ManagerOnly")]
        [HttpGet("all")]
        public async Task<ActionResult> GetAllOrders([FromQuery] AdminOrderParams adminOrderParams)
        {
            var orders = await _unitOfWork.OrderRepository.GetOrdersAsync(adminOrderParams);

            Response.AddPaginationHeader(orders.CurrentPage, orders.PageSize, orders.TotalCount, orders.TotalPages);

            var result = _mapper.Map<List<AdminOrderSummaryResponse>>(orders.ToList());

            return Ok(result);

        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpGet("summary")]
        public async Task<ActionResult> GetOrdersSummary([FromQuery] AdminOrderParams adminOrderParams)
        {
            var orderSummaries = await _unitOfWork.OrderRepository.GetOrdersSummaryAsync(adminOrderParams);

            var result = _mapper.Map<List<AdminOrderCountResponse>>(orderSummaries.ToList());

            return Ok(result);

        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpGet("{orderId}/detail")]
        public async Task<ActionResult> GetOrderDetail(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderWithDetailByIdAsync(orderId);

            if (order == null)
                return NotFound("Order not found");

            var result = _mapper.Map<AdminOrderResponse>(order);

            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAllBy(x => x.OrderId == orderId);

            var options = await _unitOfWork.ProductOptionRepository.GetAllBy(x => orderDetails.Select(x => x.OptionId).Contains(x.Id));

            foreach (var orderDetail in result.OrderDetails)
            {
                var stockQuantity = options.FirstOrDefault(x => x.Id == orderDetail.OptionId).Stock;
                orderDetail.StockAvailable = stockQuantity;
                orderDetail.StockAfterDeduction = stockQuantity - orderDetail.Quantity;
            }

            return Ok(result);
        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpPut("{orderId}/verify")]
        public async Task<ActionResult> CheckingOrder(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.Id == orderId);

            if (order == null)
                return BadRequest("Order not found");

            if (order.CurrentStatus != OrderStatus.Checking)
                return BadRequest("Can not verify order!");

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = orderId,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.Processing,
                HistoryDescription = $"Order checked by: {User.GetUsername()} at {DateTime.UtcNow.AddMonths(3)}."
            });

            order.CurrentStatus = OrderStatus.Processing;
            order.AddUpdateInformation(GetUserId());

            _unitOfWork.OrderRepository.Update(order);
            await UpdateStockQuantity(orderId, order, true);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Can not verify orders.");
        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpPut("{orderId}/shipping")]
        public async Task<ActionResult> ShippingOrder(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.Id == orderId);

            if (order == null)
                return BadRequest("Order not found");

            if (order.CurrentStatus != OrderStatus.Processing)
                return BadRequest("Can not shipping order!");

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = orderId,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.Shipping,
                HistoryDescription = $"Move to shipping process by: {User.GetUsername()} at {DateTime.UtcNow.AddMonths(3)}. Shipping method: {order.ShippingMethod}"
            });

            order.CurrentStatus = OrderStatus.Shipping;
            order.AddUpdateInformation(GetUserId());

            _unitOfWork.OrderRepository.Update(order);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Can not shipping orders.");
        }

        // [Authorize(Policy = "LogisticOnly")]
        [Authorize(Policy = "ManagerOnly")]
        [HttpPut("{orderId}/shipped")]
        public async Task<ActionResult> OrderShipped(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.Id == orderId);

            if (order == null)
                return BadRequest("Order not found");

            if (order.CurrentStatus != OrderStatus.Shipping)
                return BadRequest("Can not move order to shipped!");

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = orderId,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.Shipped,
                HistoryDescription = $"Shipped to customer by: {User.GetUsername()} at {DateTime.UtcNow.AddMonths(3)}. Shipping method: {order.ShippingMethod}"
            });

            order.CurrentStatus = OrderStatus.Shipped;
            order.AddUpdateInformation(GetUserId());

            _unitOfWork.OrderRepository.Update(order);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Can not shipping orders.");
        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpPut("{orderId}/cancel")]
        public async Task<ActionResult> CancelOrder(int orderId, CancelOrderRequest cancelOrderRequest)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.Id == orderId);

            if (order == null)
                return BadRequest("Order not found");

            var allowCancelStatus = new List<OrderStatus>()
            {
                OrderStatus.Created,
                OrderStatus.Checking,
                OrderStatus.Paid,
                OrderStatus.Processing,
                OrderStatus.CancelRequested,
            };

            if (!allowCancelStatus.Contains(order.CurrentStatus))
                return BadRequest("Can not cancel this order.");

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.Cancelled,
                HistoryDescription = $"Order is canceled by: {User.GetUsername()} at {DateTime.UtcNow.AddMonths(3)}; Reason: {cancelOrderRequest.Reason}"
            });

            order.CurrentStatus = OrderStatus.Cancelled;
            order.AddUpdateInformation(GetUserId());

            await UpdateStockQuantity(orderId, order, false);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Can not checking this order.");
        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpPut("{orderId}/return-accepted")]
        public async Task<ActionResult> AcceptOrderReturnRequest(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.Id == orderId);

            if (order == null)
                return BadRequest("Order not found");

            if (order.CurrentStatus != OrderStatus.ReturnRequested)
                return BadRequest("Can not accept to return this order.");

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.Returned,
                HistoryDescription = $"Order is accepted to return by: {User.GetUsername()} at {DateTime.UtcNow.AddMonths(3)}"
            });

            order.CurrentStatus = OrderStatus.Returned;
            order.AddUpdateInformation(GetUserId());

            await UpdateStockQuantity(orderId, order, false);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Can not checking this order.");
        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpPut("{orderId}/cancel-accepted")]
        public async Task<ActionResult> AcceptOrderCancelRequest(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstBy(x => x.Id == orderId);

            if (order == null)
                return BadRequest("Order not found");

            if (order.CurrentStatus != OrderStatus.CancelRequested)
                return BadRequest("Can not accept to cancel this order.");

            _unitOfWork.OrderHistoryRepository.Insert(new OrderHistory
            {
                OrderId = order.Id,
                CreatedByUserId = GetUserId(),
                DateCreated = DateTime.UtcNow.AddMonths(3),
                OrderStatus = OrderStatus.Cancelled,
                HistoryDescription = $"Order is accepted to cancel by: {User.GetUsername()} at {DateTime.UtcNow.AddMonths(3)}"
            });

            order.CurrentStatus = OrderStatus.Cancelled;
            order.AddUpdateInformation(GetUserId());

            await UpdateStockQuantity(orderId, order, false);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Can not checking this order.");
        }

        #endregion

        #region private method
        private async Task UpdateStockQuantity(int orderId, Order order, bool isDeduction)
        {
            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAllBy(x => x.OrderId == orderId);

            var optionIds = orderDetails.Select(x => x.OptionId);

            var options = await _unitOfWork.ProductOptionRepository.GetAllBy(x => optionIds.Contains(x.Id));

            foreach (var option in options)
            {
                var quantity = order.OrderDetails.FirstOrDefault(x => x.OptionId == option.Id).Quantity;
                if (isDeduction)
                    option.Stock -= quantity;
                else
                    option.Stock += quantity;
                option.AddUpdateInformation(GetUserId());
            }

            _unitOfWork.ProductOptionRepository.Update(options);
        }

        #endregion
    }
}