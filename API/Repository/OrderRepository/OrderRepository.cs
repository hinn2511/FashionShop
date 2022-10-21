using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Params;
using API.Entities;
using API.Entities.OrderModel;
using API.Extensions;
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

        public Task<PagedList<Order>> GetOrdersAsAdminAsync(AdminOrderParams orderParams)
        {
            throw new System.NotImplementedException();
        }

        public async Task<PagedList<Order>> GetOrdersAsCustomerAsync(CustomerOrderParams orderParams, int userId)
        {
            var query = _context.Orders.AsQueryable();

            query = query.Where(x => x.CreatedByUserId == userId);

            query = query.Where(x => x.Status != Status.Hidden && x.Status != Status.Deleted);

            if (orderParams.OrderStatus != null)
                query = query.Where(x => orderParams.OrderStatus == x.CurrentStatus);

            if(!string.IsNullOrEmpty(orderParams.Query))
            {   
                var words = orderParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach(var word in words)
                {
                    query = query.Where(x => x.ExternalId.ToUpper().Contains(word));
                }
            }

            if(orderParams.From != DateTime.MinValue)
                query = query.Where(x => x.DateCreated >= orderParams.From);

            if(orderParams.From != DateTime.MinValue)
                query = query.Where(x => x.DateCreated <= orderParams.To);


            if (orderParams.OrderBy == OrderBy.Ascending) 
            {
                query = orderParams.Field switch
                {
                    "DateCreated" => query.OrderBy(p => p.DateCreated),
                    _ => query.OrderBy(p => p.ExternalId)
                };
            }
            else
            {
                query = orderParams.Field switch
                {
                    "DateCreated" => query.OrderByDescending(p => p.DateCreated),
                    _ => query.OrderByDescending(p => p.ExternalId)
                };
            }

            query = query.Include(x => x.OrderDetails).ThenInclude(x => x.Option).ThenInclude(x => x.Product);

            return await PagedList<Order>.CreateAsync(query.AsNoTracking(), orderParams.PageNumber, orderParams.PageSize);
        }

        public async Task<PagedList<Order>> GetOrdersAsync(AdminOrderParams orderParams)
        {
            var query = _context.Orders.AsQueryable();

            

            return await PagedList<Order>.CreateAsync(query, orderParams.PageNumber, orderParams.PageSize);
        }

        public async Task<Order> GetOrderWithDetailByIdAsync(int orderId)
        {
            return await _context.Orders
                        .Where(x => x.Id == orderId)
                        .Include(x => x.OrderHistories)
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Option)
                        .ThenInclude(x => x.Product)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
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