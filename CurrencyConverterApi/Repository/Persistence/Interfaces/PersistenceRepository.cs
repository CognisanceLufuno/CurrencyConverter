using System.Linq.Expressions;

namespace CurrencyConverterApi.Repository.Persistence.Interfaces
{
    public interface IPersistenceRepository<T> where T : class
    { 
        Task<T> GetById(int id);
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> SearchBy(Expression<Func<T, bool>> expression);
        Task<T> SearchElementBy(Expression<Func<T, bool>> expression);
        Task<T> AddAsync(T entity);
        T Update(T entity);
    }
}
