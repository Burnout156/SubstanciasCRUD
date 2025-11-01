using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasDatabase.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext appDbContextRead;
        public GenericRepository(AppDbContext appDbContext) => this.appDbContextRead = appDbContext;

        public Task<List<T>> GetAllAsync(CancellationToken cancellationToken) => appDbContextRead.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
        public Task<T?> GetAsync(int id, CancellationToken cancellationToken) => appDbContextRead.Set<T>().FindAsync([id], cancellationToken).AsTask();
        public Task AddAsync(T entity, CancellationToken cancellationToken) { appDbContextRead.Set<T>().Add(entity); return Task.CompletedTask; }
        public Task UpdateAsync(T entity, CancellationToken cancellationToken) { appDbContextRead.Set<T>().Update(entity); return Task.CompletedTask; }
        public Task DeleteAsync(T entity, CancellationToken cancellationToken) { appDbContextRead.Set<T>().Remove(entity); return Task.CompletedTask; }
        public Task SaveAsync(CancellationToken cancellationToken) => appDbContextRead.SaveChangesAsync(cancellationToken);
    }
}
