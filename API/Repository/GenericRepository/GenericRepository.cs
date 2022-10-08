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

        public void BulkInsert(IEnumerable<T> objs)
        {
            table.AddRange(objs);
        }

        public void Update(T obj)
        {
            table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }

        public void BulkUpdate(IEnumerable<T> objs)
        {
            table.AttachRange(objs);
            foreach (var obj in objs)
                _context.Entry(obj).State = EntityState.Modified;
        }


        public void Delete(int id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }

        public void BulkDelete(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                T existing = table.Find(id);
                table.RemoveRange(existing);
            }
        }

        public async Task<T> GetFirstBy(Expression<Func<T, bool>> expression)
        {
            return await table.FirstOrDefaultAsync(expression);
        }
    }
}