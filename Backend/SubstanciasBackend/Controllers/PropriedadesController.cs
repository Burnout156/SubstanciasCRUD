using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubstanciasDatabase.Repositories;
using SubstanciasLibrary.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SubstanciasBackend.Controllers
{
    [ApiController]
    [Route("api/propriedades")]
    [Authorize]
    public class PropriedadesController : ControllerBase
    {
        private readonly IGenericRepository<Propriedade> _repo;
        public PropriedadesController(IGenericRepository<Propriedade> repo) => _repo = repo;

        [HttpGet] public async Task<IActionResult> All(CancellationToken ct) => Ok(await _repo.GetAllAsync(ct));
        [HttpPost] public async Task<IActionResult> Create([FromBody] Propriedade p, CancellationToken ct) { 
            await _repo.AddAsync(p, ct); await _repo.SaveAsync(ct); return Ok(p); 
        }
    }
}
