using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SecchiServicios.Data.Data;
using SecchiServicios.Data.Interfaces;

namespace SecchiServicios.Data.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class
{
    
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }
    
    public Task<List<T>> GetAllAsync()
        => _dbSet.ToListAsync();
    
    public virtual IQueryable<T> Query()
        => _dbSet.AsQueryable();
    

    public async Task AddAsync(T item)
    {
        await _dbSet.AddAsync(item);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        var itemsToDelete = await _dbSet.Where(predicate).ToListAsync();

        if (itemsToDelete.Any())
        {
            _dbSet.RemoveRange(itemsToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task UpdateRangeAsync(IEnumerable<T> items, Func<T, object> keySelector)
    {
        foreach (var item in items)
        {
            var key = keySelector(item);
            var trackedItem = _dbSet.Find(key);

            if (trackedItem != null)
            {
                _dbContext.Entry(trackedItem).CurrentValues.SetValues(item);
            }
            else
            {
                _dbSet.Attach(item);
                _dbContext.Entry(item).State = EntityState.Modified;
            }
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public Task<List<T>> GetByPredicateAsync(Expression<Func<T, bool>> predicate)
        =>  _dbSet.Where(predicate).ToListAsync();
}