using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities.OrderModel;

namespace API.Repository.OrderRepository
{
    public interface IOrderRepository
    {
        #region order
        Task<Order> GetOrderByIdAsync(int id);
        void Create(Order order);
        void Update(Order order);
        void Delete(Order order);

        #endregion

        #region order detail
        void BulkCreate(List<OrderDetail> orderDetails);
        Task<IEnumerable<OrderDetail>> GetByOrderId(int orderId);

        #endregion

        #region order history
        void Create(OrderHistory orderHistory);

        #endregion
    }
}