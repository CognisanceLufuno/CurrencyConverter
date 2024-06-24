using CurrencyConverterApi.Repository.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CurrencyConverterApi.Repository.Persistence
{
    public class PersistenceRepository<T> : IPersistenceRepository<T> where T : class
    {
        protected readonly DBContext _context;
        public PersistenceRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            _context.Set<T>().AddAsync(entity);
            return entity;
        }
        public async Task<T> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<IEnumerable<T>> SearchBy(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).ToListAsync();
        }

        public async Task<T> SearchElementBy(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).FirstOrDefaultAsync();
        }

        public T Update(T entity)
        {
            _context.Entry<T>(entity).State = EntityState.Modified;
            _context.Set<T>().Update(entity);
            return entity;
        }
    }
}
