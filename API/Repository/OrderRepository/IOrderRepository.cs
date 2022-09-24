using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities.OrderModel;
using API.Helpers;
using API.Repository.GenericRepository;

namespace API.Repository.OrderRepository
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<PagedList<Order>> GetOrdersAsync(OrderParams orderParams);
    }

    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {

    }

    public interface IOrderHistoryRepository : IGenericRepository<OrderHistory>
    {

    }
}