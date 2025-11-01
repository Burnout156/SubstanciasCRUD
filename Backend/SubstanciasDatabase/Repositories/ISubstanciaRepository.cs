using SubstanciasLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasDatabase.Repositories
{
    public interface ISubstanciaRepository
    {
        Task<(IEnumerable<Substancia> items, int total)> 
            ListAsync(string? search, int page, int pageSize, CancellationToken cancellationToken);
        Task<Substancia?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Substancia?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken);
        Task AddAsync(Substancia substancia, CancellationToken cancellationToken);
        Task UpdateAsync(Substancia substancia, CancellationToken cancellationToken);
        Task DeleteAsync(Substancia substancia, CancellationToken cancellationToken);
        Task SaveAsync(CancellationToken cancellationToken);
    }
}
