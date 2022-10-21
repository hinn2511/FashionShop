using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities.OrderModel;
using API.DTOs.Params;
using API.Repository.GenericRepository;
using API.Helpers;

namespace API.Repository.OrderRepository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<PagedList<Order>> GetOrdersAsCustomerAsync(CustomerOrderParams orderParams, int userId);
        Task<PagedList<Order>> GetOrdersAsAdminAsync(AdminOrderParams orderParams);
        Task<Order> GetOrderWithDetailByIdAsync(int orderId);
    }

    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {

    }

    public interface IOrderHistoryRepository : IGenericRepository<OrderHistory>
    {

    }
}