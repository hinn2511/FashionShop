using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Entities.OrderModel;
using API.Interfaces;

namespace API.Repository.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;

        public OrderRepository(DataContext context)
        {
            _context = context;
        }

        public void BulkCreate(List<OrderDetail> orderDetails)
        {
            _context.OrderDetails.AddRange(orderDetails);
        }

        public void Create(Order order)
        {
            _context.Orders.Add(order);
        }

        public void Create(OrderHistory orderHistory)
        {
            _context.OrderHistories.Add(orderHistory);
        }

        public void Delete(Order order)
        {
            _context.Orders.Remove(order);
        }

        public Task<IEnumerable<OrderDetail>> GetByOrderId(int orderId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Order> GetOrderByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }
    }
}