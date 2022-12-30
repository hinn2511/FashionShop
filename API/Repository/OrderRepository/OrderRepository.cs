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

        public async Task<PagedList<Order>> GetOrdersAsync(AdminOrderParams orderParams)
        {

            var query = _context.Orders.AsQueryable();


            if (orderParams.OrderStatusFilter.Any())
                query = query.Where(x => orderParams.OrderStatusFilter.Contains(x.CurrentStatus));

            if (orderParams.ShippingMethodFilter.Any())
                query = query.Where(x => orderParams.ShippingMethodFilter.Select(x => x.ToUpper()).Contains(x.ShippingMethod.ToUpper()));

            if (orderParams.PaymentMethodFilter.Any())
                query = query.Where(x => orderParams.PaymentMethodFilter.Contains(x.PaymentMethod));

            if (orderParams.From != DateTime.MinValue)
            {
                var from = new DateTime(orderParams.From.Year, orderParams.From.Month, orderParams.From.Day, 0, 0, 0);
                query = query.Where(x => x.DateCreated >= from);

            }

            if (orderParams.To != DateTime.MinValue)
            {
                var to = new DateTime(orderParams.To.Year, orderParams.To.Month, orderParams.To.Day, 23, 59, 59);
                query = query.Where(x => x.DateCreated <= to);
            }

            if (orderParams.To != DateTime.MinValue)
            {
                var to = new DateTime(orderParams.To.Year, orderParams.To.Month, orderParams.To.Day, 23, 59, 59);
                query = query.Where(x => x.DateCreated <= to);
            }

            if (!string.IsNullOrEmpty(orderParams.Query))
            {
                var words = orderParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach (var word in words)
                {
                    query = query.Where(x => x.ExternalId.ToUpper().Contains(word)
                    || x.User.FirstName.ToUpper().Contains(word)
                    || x.User.LastName.ToUpper().Contains(word)
                    || x.User.PhoneNumber.Contains(word)
                    );
                }
            }

            if (orderParams.OrderBy == OrderBy.Ascending)
            {
                query = orderParams.Field switch
                {
                    "DateCreated" => query.OrderBy(o => o.DateCreated),
                    "TotalPrice" => query.OrderBy(o => o.SubTotal + o.Tax + o.ShippingFee),
                    "TotalQuantity" => query.OrderBy(o => o.OrderDetails.Count),
                    "FirstName" => query.OrderBy(o => o.User.FirstName),
                    "LastName" => query.OrderBy(o => o.User.LastName),
                    "ExternalId" => query.OrderBy(o => o.ExternalId),
                    "Status" => query.OrderBy(o => o.CurrentStatus),
                    "PaymentMethod" => query.OrderBy(o => o.PaymentMethod),
                    "ShippingMethod" => query.OrderBy(o => o.ShippingMethod),
                    _ => query.OrderBy(o => o.Id)
                };
            }
            else
            {
                query = orderParams.Field switch
                {
                    "DateCreated" => query.OrderByDescending(o => o.DateCreated),
                    "TotalPrice" => query.OrderByDescending(o => o.SubTotal + o.Tax + o.ShippingFee),
                    "TotalQuantity" => query.OrderByDescending(o => o.OrderDetails.Count),
                    "FirstName" => query.OrderByDescending(o => o.User.FirstName),
                    "LastName" => query.OrderByDescending(o => o.User.LastName),
                    "ExternalId" => query.OrderByDescending(o => o.ExternalId),
                    "Status" => query.OrderByDescending(o => o.CurrentStatus),
                    "PaymentMethod" => query.OrderByDescending(o => o.PaymentMethod),
                    "ShippingMethod" => query.OrderByDescending(o => o.ShippingMethod),
                    _ => query.OrderByDescending(o => o.Id)
                };
            }

            query = query.Include(x => x.OrderDetails).ThenInclude(x => x.Option).ThenInclude(x => x.Product).Include(x => x.User);


            return await PagedList<Order>.CreateAsync(query.AsNoTracking(), orderParams.PageNumber, orderParams.PageSize);
        }

        public async Task<PagedList<Order>> GetOrdersAsync(CustomerOrderParams orderParams, int userId)
        {
            var query = _context.Orders.AsQueryable();

            query = query.Where(x => x.CreatedByUserId == userId);

            query = query.Where(x => x.Status == Status.Active);

            if (orderParams.OrderStatusFilter.Any())
                query = query.Where(x => orderParams.OrderStatusFilter.Contains(x.CurrentStatus));

            if (!string.IsNullOrEmpty(orderParams.Query))
            {
                var words = orderParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach (var word in words)
                {
                    query = query.Where(x => x.ExternalId.ToUpper().Contains(word));
                }
            }

            if (orderParams.From != DateTime.MinValue)
            {
                var from = new DateTime(orderParams.From.Year, orderParams.From.Month, orderParams.From.Day, 0, 0, 0);
                query = query.Where(x => x.DateCreated >= from);

            }

            if (orderParams.To != DateTime.MinValue)
            {
                var to = new DateTime(orderParams.To.Year, orderParams.To.Month, orderParams.To.Day, 23, 59, 59);
                query = query.Where(x => x.DateCreated <= to);
            }


            if (orderParams.OrderBy == OrderBy.Ascending)
            {
                query = orderParams.Field switch
                {
                    "Date" => query.OrderBy(p => p.DateCreated),
                    "Total" => query.OrderBy(p => p.SubTotal + p.Tax + p.ShippingFee),
                    _ => query.OrderBy(p => p.ExternalId)
                };
            }
            else
            {
                query = orderParams.Field switch
                {
                    "Date" => query.OrderByDescending(p => p.DateCreated),
                    "Total" => query.OrderByDescending(p => p.SubTotal + p.Tax + p.ShippingFee),
                    _ => query.OrderByDescending(p => p.ExternalId)
                };
            }

            query = query.Include(x => x.OrderHistories)
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Option)
                        .ThenInclude(x => x.Color)
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Option)
                        .ThenInclude(x => x.Size)
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Option)
                        .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.ProductPhotos);
                        
            return await PagedList<Order>.CreateAsync(query.AsNoTracking(), orderParams.PageNumber, orderParams.PageSize);
        }

        public Task<List<Tuple<OrderStatus, int>>> GetOrdersSummaryAsync()
        {
            var result = _context.Orders.GroupBy(x => x.CurrentStatus).Select(g => Tuple.Create(g.Key,g.Count())).ToListAsync();
            return result;
        }

        public async Task<Order> GetOrderWithDetailByExternalIdAsync(string externalId)
        {
            return await _context.Orders
                        .Where(x => x.ExternalId == externalId)
                        .Include(x => x.OrderHistories)
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Option)
                        .ThenInclude(x => x.Color)
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Option)
                        .ThenInclude(x => x.Size)
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Option)
                        .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.ProductPhotos)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrderWithDetailByIdAsync(int orderId)
        {
            return await _context.Orders
                        .Where(x => x.Id == orderId)
                        .Include(x => x.OrderHistories)
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Option)
                        .ThenInclude(x => x.Color)
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Option)
                        .ThenInclude(x => x.Size)
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Option)
                        .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.ProductPhotos)
                        .Include(x => x.User)
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