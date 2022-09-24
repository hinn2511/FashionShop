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
        void Insert(T obj);
        void Update(T obj);
        void Delete(object id);
    }
}