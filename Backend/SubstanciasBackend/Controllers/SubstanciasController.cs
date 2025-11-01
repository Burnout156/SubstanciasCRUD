using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubstanciasLibrary.Models.Enums;
using SubstanciasDatabase.Services;
using SubstanciasLibrary.Dtos;

namespace Substances.Api.Controllers
{
    [ApiController]
    [Route("api/substancias")]
    [Authorize] // exige token do Keycloak
    public class SubstanciasController : ControllerBase
    {
        private readonly ISubstanciaService _svc;
        public SubstanciasController(ISubstanciaService svc) => _svc = svc;

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var (items, total) = await _svc.ListAsync(search, page, pageSize, ct);

            var result = items.Select(s => new SubstanciaListItemDto(
                s.Id, s.Codigo, s.Nome, s.Descricao, s.Notas, s.CategoriaId, s.Categoria.Nome
            ));

            return Ok(new { total, items = result });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id, CancellationToken ct)
        {
            var s = await _svc.GetAsync(id, ct);
            if (s is null) return NotFound();

            var dto = new SubstanciaDetailDto(
                s.Id, s.Codigo, s.Nome, s.Descricao, s.Notas,
                s.CategoriaId, s.Categoria.Nome,
                s.SubstanciaPropriedades.Select(sp => (sp.PropriedadeId, sp.Propriedade.Nome, sp.ValueType, sp.BoolValue, sp.DecimalValue)).ToList()
            );

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubstanciaCreateDto dto, CancellationToken ct)
        {
            var s = await _svc.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(Get), new { id = s.Id }, new { s.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SubstanciaUpdateDto dto, CancellationToken ct)
        {
            var s = await _svc.UpdateAsync(id, dto, ct);
            if (s is null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _svc.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
