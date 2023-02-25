using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.DTOs.Response.DashboardResponse;
using API.Entities.OrderModel;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class DashboardController : BaseApiController
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public DashboardController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpGet("order-status")]
        public async Task<ActionResult> GetDashboardOrderStatusSummary([FromQuery] ChartParams chartParams)
        {
            var param = new AdminOrderParams();
            param.From = chartParams.From;
            param.To = chartParams.To;

            var orderSummaries = await _unitOfWork.OrderRepository.GetOrdersSummaryAsync(param);

            var result = orderSummaries.Select(x => new ReportResponse(x.Item1.ConvertToString(), x.Item2)).ToList();

            return Ok(result);

        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpGet("popular-product")]
        public async Task<ActionResult> GetPopularProduct([FromQuery] ChartParams chartParams)
        {
            var param = new AdminOrderParams();
            param.From = chartParams.From;
            param.To = chartParams.To;

            var orders = await _unitOfWork.OrderRepository.GetOrdersAsync(param);

            var orderDetails = orders.SelectMany(x => x.OrderDetails).ToList();

            var result = orderDetails.GroupBy(x => x.Option.Product.ProductName)
             .Select(g => new ReportResponse(g.Key, g.Count())).OrderByDescending(g => g.Value).Take(chartParams.Limit);

            return Ok(result);

        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpGet("order-rate")]
        public async Task<ActionResult> GetOrderRate([FromQuery] ChartParams chartParams)
        {
            var param = new AdminOrderParams();
            param.From = chartParams.From;
            param.To = chartParams.To;

            var orders = await _unitOfWork.OrderRepository.GetOrdersAsync(param);

            var result = new List<ReportResponse>();

            var successOrders = orders.Where(x => x.CurrentStatus == OrderStatus.Finished).Count();

            var returnOrders = orders.Where(x => x.CurrentStatus == OrderStatus.Returned || x.CurrentStatus == OrderStatus.ReturnRequested).Count();

            var cancelledOrders = orders.Where(x => x.CurrentStatus == OrderStatus.CancelRequested || x.CurrentStatus == OrderStatus.Cancelled).Count();

            var declineOrders = orders.Where(x => x.CurrentStatus == OrderStatus.Declined).Count();

            result.Add(new ReportResponse(
                "Completion rate",
                CalculateRate(successOrders, orders.Count())
            ));

            result.Add(new ReportResponse(
                "Returning rate",
                CalculateRate(returnOrders, orders.Count())
            ));

            result.Add(new ReportResponse(
               "Canceling rate",
               CalculateRate(cancelledOrders, orders.Count())
           ));


            result.Add(new ReportResponse(
                "Declining rate",
                CalculateRate(declineOrders, orders.Count())
            ));

            return Ok(result);

        }

        [Authorize(Policy = "ManagerOnly")]
        [HttpGet("revenue")]
        public async Task<ActionResult> GetRevenue([FromQuery] ChartParams chartParams)
        {
            var param = new AdminOrderParams();
            var now = DateTime.UtcNow;
            switch (chartParams.Metric)
            {
                case "day":
                    {
                        param.From = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                        param.To = param.From.AddMonths(1).AddSeconds(-1);
                        break;
                    }
                default:
                    {
                        param.From = new DateTime(DateTime.UtcNow.Year, 1, 1);
                        param.To = param.From.AddYears(1).AddSeconds(-1);
                        break;
                    }
            }

            param.OrderStatusFilter = new List<OrderStatus>() { OrderStatus.Finished };

            var orders = await _unitOfWork.OrderRepository.GetOrdersAsync(param);

            var result = new List<ReportResponse>();

            switch (chartParams.Metric)
            {
                case "day":
                    {
                        result = orders.GroupBy(x => x.DateCreated.Date).Select(x => new ReportResponse(x.Key.ToLongDateString(), x.Sum(x => x.SubTotal))).ToList();
                        break;
                    }
                case "month":
                    {
                        result = orders.GroupBy(x => x.DateCreated.Month)
                        .OrderBy(x => x.Key)
                        .Select(x => new ReportResponse(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Key), x.Sum(x => x.SubTotal))).ToList();
                        break;
                    }
                case "quarter":
                    {
                        result = orders
                            .GroupBy(x => new { Quarter = x.DateCreated.Month.GetQuarter(), x.DateCreated.Year })
                            .OrderBy(x => x.Key.Quarter)
                            .Select(x => new ReportResponse($"Q{x.Key.Quarter}/{x.Key.Year}".ToString(),
                                         x.Sum(x => x.SubTotal + x.Tax))).ToList();
                        break;
                    }
                default:
                    {
                        result = orders.GroupBy(x => x.DateCreated.Year).Select(x => new ReportResponse(x.Key.ToString(), x.Sum(x => x.SubTotal + x.Tax))).ToList();
                        break;
                    }
            }

            return Ok(result);

        }

        #region private method 

        private int CalculateRate(int value, int total)
        {
            return Convert.ToInt32(((decimal)value / (decimal)total) * 100);
        }


        #endregion

    }
}