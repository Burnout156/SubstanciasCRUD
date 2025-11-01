using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubstanciasDatabase.Repositories;
using SubstanciasLibrary.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SubstanciasBackend.Controllers
{
    [ApiController]
    [Route("api/categorias")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly IGenericRepository<Categoria> _repo;
        public CategoriasController(IGenericRepository<Categoria> repo) => _repo = repo;

        [HttpGet] 
        public async Task<IActionResult> All(CancellationToken ct) => Ok(await _repo.GetAllAsync(ct));

        [HttpPost] 
        public async Task<IActionResult> Create([FromBody] Categoria c, CancellationToken ct) {
            await _repo.AddAsync(c, ct); await _repo.SaveAsync(ct); return Ok(c); 
        }
    }
}
