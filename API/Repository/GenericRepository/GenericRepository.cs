using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private DataContext _context = null;
        private DbSet<T> table = null;
        public GenericRepository(DataContext context, DbSet<T> set)
        {
            _context = context;
            table = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await table.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllBy(Expression<Func<T, bool>> expression)
        {
            return await table.Where(expression).ToListAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await table.FindAsync(id);
        }

        public void Insert(T obj)
        {
            table.Add(obj);
        }

        public void Insert(IEnumerable<T> objs)
        {
            table.AddRange(objs);
        }

        public void Update(T obj)
        {
            table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }

        public void Update(IEnumerable<T> objs)
        {
            table.AttachRange(objs);
            foreach (var obj in objs)
                _context.Entry(obj).State = EntityState.Modified;
        }


        public void Delete(object id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }

        public void Delete(Expression<Func<T, bool>> expression)
        {
            var deleted = table.Where(expression);
            table.RemoveRange(deleted);
        }

        public async Task<T> GetFirstBy(Expression<Func<T, bool>> expression)
        {
            return await table.FirstOrDefaultAsync(expression);
        }

        public async Task<IEnumerable<T>> GetAllAndIncludeAsync(Expression<Func<T, bool>> filter, string includeProperties, bool isNoTracking)
        {
            IQueryable<T> query = table;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (isNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetFirstByAndIncludeAsync(Expression<Func<T, bool>> filter, string includeProperties, bool isNoTracking)
        {
            IQueryable<T> query = table;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (isNoTracking)
            {
                query = query.AsNoTracking();
            }


            return await query.FirstOrDefaultAsync();
        }
    }
}