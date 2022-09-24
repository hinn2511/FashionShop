using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities.OrderModel;
using API.Helpers;
using API.Interfaces;
using API.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.OrderRepository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext _context;

        public OrderRepository(DataContext context, DbSet<Order> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<PagedList<Order>> GetOrdersAsync(OrderParams orderParams)
        {
            var query = _context.Orders.AsQueryable();

            query.Where(o => o.DateCreated >= orderParams.From && o.DateCreated <= orderParams.To);
            
            if(orderParams.Id != null)
                query = query.Where(p => p.Id == orderParams.Id);

            if(orderParams.OrderStatus != null)
                query = query.Where(p => p.CurrentStatus == orderParams.OrderStatus);

            if(orderParams.CreatedByUserId != null)
                query = query.Where(p => p.CreatedByUserId == orderParams.CreatedByUserId);

            query = orderParams.OrderBy switch
            {
                OrderBy.Newest => query.OrderByDescending(p => p.OrderHistories.OrderBy(x => x.Id).Last().DateCreated),
                OrderBy.Oldest => query.OrderBy(p => p.OrderHistories.OrderBy(x => x.Id).Last().DateCreated),
                _ => query.OrderByDescending(p => p.OrderHistories.OrderBy(x => x.Id).Last().DateCreated)
            };

            return await PagedList<Order>.CreateAsync(query, orderParams.PageNumber, orderParams.PageSize);
        }


    }

    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(DataContext context, DbSet<OrderDetail> set) : base(context, set)
        {
        }
    }

    public class OrderHistoryRepository : GenericRepository<OrderHistory>, IOrderHistoryRepository
    {
        public OrderHistoryRepository(DataContext context, DbSet<OrderHistory> set) : base(context, set)
        {
        }
    }
}