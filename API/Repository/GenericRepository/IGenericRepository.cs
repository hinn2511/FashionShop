using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repository.GenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(object id);
        Task<IEnumerable<T>> GetAllBy(Expression<Func<T, bool>> expression);
        Task<T> GetFirstBy(Expression<Func<T, bool>> expression);
        Task<T> GetFirst();

        Task<IEnumerable<T>> GetAllAndIncludeAsync(
            Expression<Func<T, bool>> filter,
            string includeProperties, bool isNoTracking);

        Task<T> GetFirstByAndIncludeAsync(
            Expression<Func<T, bool>> filter,
            string includeProperties, bool isNoTracking);

        void Insert(T obj);
        void Update(T obj);
        void Delete(object id);

        void Insert(IEnumerable<T> objs);
        void Update(IEnumerable<T> objs);
        void Delete(Expression<Func<T, bool>> expression);
    }
}