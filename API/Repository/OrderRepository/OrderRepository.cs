using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Params;
using API.Entities;
using API.Entities.OrderModel;
using API.Entities.UserModel;
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
                    "dateCreated" => query.OrderBy(o => o.DateCreated),
                    "totalPrice" => query.OrderBy(o => o.SubTotal + o.Tax + o.ShippingFee),
                    "totalQuantity" => query.OrderBy(o => o.OrderDetails.Count),
                    "firstName" => query.OrderBy(o => o.User.FirstName),
                    "lastName" => query.OrderBy(o => o.User.LastName),
                    "externalId" => query.OrderBy(o => o.ExternalId),
                    "status" => query.OrderBy(o => o.CurrentStatus),
                    "paymentMethod" => query.OrderBy(o => o.PaymentMethod),
                    "shippingMethod" => query.OrderBy(o => o.ShippingMethod),
                    _ => query.OrderBy(o => o.Id)
                };
            }
            else
            {
                query = orderParams.Field switch
                {
                    "dateCreated" => query.OrderByDescending(o => o.DateCreated),
                    "totalPrice" => query.OrderByDescending(o => o.SubTotal + o.Tax + o.ShippingFee),
                    "totalQuantity" => query.OrderByDescending(o => o.OrderDetails.Count),
                    "firstName" => query.OrderByDescending(o => o.User.FirstName),
                    "lastName" => query.OrderByDescending(o => o.User.LastName),
                    "externalId" => query.OrderByDescending(o => o.ExternalId),
                    "status" => query.OrderByDescending(o => o.CurrentStatus),
                    "paymentMethod" => query.OrderByDescending(o => o.PaymentMethod),
                    "shippingMethod" => query.OrderByDescending(o => o.ShippingMethod),
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
                    "date" => query.OrderBy(p => p.DateCreated),
                    "total" => query.OrderBy(p => p.SubTotal + p.Tax + p.ShippingFee),
                    _ => query.OrderBy(p => p.ExternalId)
                };
            }
            else
            {
                query = orderParams.Field switch
                {
                    "date" => query.OrderByDescending(p => p.DateCreated),
                    "total" => query.OrderByDescending(p => p.SubTotal + p.Tax + p.ShippingFee),
                    _ => query.OrderByDescending(p => p.ExternalId)
                };
            }

            query = query.Include(x => x.OrderHistories)
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Option)
                        .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.Option)
                        .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.ProductPhotos);
                        
            return await PagedList<Order>.CreateAsync(query.AsNoTracking(), orderParams.PageNumber, orderParams.PageSize);
        }

        public Task<List<Tuple<OrderStatus, int>>> GetOrdersSummaryAsync(AdminOrderParams orderParams)
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

            var result = query.GroupBy(x => x.CurrentStatus).Select(g => Tuple.Create(g.Key,g.Count())).ToListAsync();

            return result;
        }

        public async Task<Order> GetOrderWithDetailByExternalIdAsync(string externalId)
        {
            return await _context.Orders
                        .Where(x => x.ExternalId == externalId)
                        .Include(x => x.OrderHistories)
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

    public class UserReviewRepository : GenericRepository<UserReview>, IUserReviewRepository
    {
        private readonly DataContext _context;

        public UserReviewRepository(DataContext context, DbSet<UserReview> set) : base(context, set)
        {
            _context = context;
        }

        public async Task<PagedList<UserReview>> GetProductReviewsAsync(int productId, CustomerReviewParams customerReviewParams)
        {
            var options = _context.Options.Where(x => x.ProductId == productId).Select(x => x.Id);

            var query = _context.UserReviews.AsQueryable();

            query = query.Where(x => options.Contains(x.OptionId));

            if ( customerReviewParams.Score > 0 && customerReviewParams.Score <= 5)
            query = query.Where(x => x.Score == customerReviewParams.Score);

            if (customerReviewParams.OrderBy == OrderBy.Ascending)
            {
                query = customerReviewParams.Field switch
                {
                    "dateCreated" => query.OrderBy(p => p.DateCreated),
                    "score" => query.OrderBy(p => p.Score),
                    _ => query.OrderBy(p => p.DateCreated)
                };
            }
            else
            {
                query = customerReviewParams.Field switch
                {
                    "dateCreated" => query.OrderByDescending(p => p.DateCreated),
                    "score" => query.OrderByDescending(p => p.Score),
                    _ => query.OrderByDescending(p => p.DateCreated)
                };
            }

            query = query.Include(x => x.User).Include(x => x.Option);

            return await PagedList<UserReview>.CreateAsync(query, customerReviewParams.PageNumber, customerReviewParams.PageSize);

        }

        public async Task<IEnumerable<UserReview>> GetReviewedItemAsync(int orderId)
        {
            return await _context.UserReviews.Where(x => x.OrderId == orderId)
                                    .Include(x => x.Option)
                                    .ThenInclude(x => x.Product)
                                    .ThenInclude(x => x.ProductPhotos)
                                    .OrderByDescending(x => x.DateCreated)
                                    .ToListAsync();
        }
    }
}