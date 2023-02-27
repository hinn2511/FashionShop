using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities.OrderModel;
using API.DTOs.Params;
using API.Repository.GenericRepository;
using API.Helpers;
using System;
using API.Entities.UserModel;

namespace API.Repository.OrderRepository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<PagedList<Order>> GetOrdersAsync(CustomerOrderParams orderParams, int userId);
        Task<PagedList<Order>> GetOrdersAsync(AdminOrderParams orderParams);
        Task<Order> GetOrderWithDetailByIdAsync(int orderId);
        Task<Order> GetOrderWithDetailByExternalIdAsync(string externalId);
        Task<List<Tuple<OrderStatus, int>>> GetOrdersSummaryAsync(AdminOrderParams orderParams);
    }

    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {

    }

    public interface IOrderHistoryRepository : IGenericRepository<OrderHistory>
    {

    }

    public interface IUserReviewRepository : IGenericRepository<UserReview>
    {
        Task<PagedList<UserReview>> GetProductReviewsAsync(int productId, CustomerReviewParams customerReviewParams);

        Task<IEnumerable<UserReview>> GetReviewedItemAsync(int orderId);
    }
}