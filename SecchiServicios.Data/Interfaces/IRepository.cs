using System.Linq.Expressions;

namespace SecchiServicios.Data.Interfaces;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task AddAsync(T item);
    Task DeleteAsync(Expression<Func<T, bool>> predicate);
    Task UpdateRangeAsync(IEnumerable<T> items, Func<T, object> keySelector);
    Task<List<T>> GetByPredicateAsync(Expression<Func<T, bool>> predicate);
}