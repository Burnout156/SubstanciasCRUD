using SubstanciasLibrary.Dtos;
using SubstanciasLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasDatabase.Services
{
    public interface ISubstanciaService
    {
        Task<(IEnumerable<Substancia> items, int total)> ListAsync(string? search, int page, int pageSize, CancellationToken cancellationToken);
        Task<Substancia?> GetAsync(int id, CancellationToken cancellationToken);
        Task<Substancia> CreateAsync(SubstanciaCreateDto substanciaCreateDto, CancellationToken cancellationToken);
        Task<Substancia?> UpdateAsync(int id, SubstanciaUpdateDto substanciaUpdateDto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
