using Microsoft.EntityFrameworkCore;
using SubstanciasLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasDatabase.Repositories
{
    public class SubstanciaRepository : ISubstanciaRepository
    {
        private readonly AppDbContext appDbContext;
        public SubstanciaRepository(AppDbContext appDbContext) => this.appDbContext = appDbContext;

        public async Task<(IEnumerable<Substancia> items, int total)> ListAsync(string? search, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = appDbContext.Substancias
                .Include(substancia => substancia.Categoria)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Busca por Codigo (não criptografado) e por Nome (criptografado — não dá pra filtrar por LIKE, então mantemos busca por codigo)
                query = query.Where(substancia => substancia.Codigo.ToLower().Contains(search.ToLower()));
            }

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(substancia => substancia.Codigo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public Task<Substancia?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            appDbContext.Substancias
                .Include(substancia => substancia.Categoria)
                .Include(substancia => substancia.SubstanciaPropriedades).ThenInclude(sp => sp.Propriedade)
                .FirstOrDefaultAsync(substancia => substancia.Id == id, cancellationToken);

        public Task<Substancia?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken) =>
            appDbContext.Substancias.FirstOrDefaultAsync(substancia => substancia.Codigo == codigo, cancellationToken);

        public Task AddAsync(Substancia substancia, CancellationToken cancellationToken) { appDbContext.Add(substancia); return Task.CompletedTask; }
        public Task UpdateAsync(Substancia substancia, CancellationToken cancellationToken) { appDbContext.Update(substancia); return Task.CompletedTask; }
        public Task DeleteAsync(Substancia substancia, CancellationToken cancellationToken) { appDbContext.Remove(substancia); return Task.CompletedTask; }
        public Task SaveAsync(CancellationToken cancellationToken) => appDbContext.SaveChangesAsync(cancellationToken);
    }
}
