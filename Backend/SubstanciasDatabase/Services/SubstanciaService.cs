using Microsoft.EntityFrameworkCore;
using SubstanciasDatabase.Repositories;
using SubstanciasLibrary.Dtos;
using SubstanciasLibrary.Models;
using SubstanciasLibrary.Models.Enums; // ✅ Corrigido: agora o enum PropertyValueType é reconhecido

namespace SubstanciasDatabase.Services
{
    public class SubstanciaService : ISubstanciaService
    {
        private readonly ISubstanciaRepository _repo;
        private readonly AppDbContext _ctx;

        public SubstanciaService(ISubstanciaRepository repo, AppDbContext ctx)
        {
            _repo = repo;
            _ctx = ctx;
        }

        // 📋 Lista paginada de substâncias
        public Task<(IEnumerable<Substancia> items, int total)> ListAsync(string? search, int page, int pageSize, CancellationToken ct)
            => _repo.ListAsync(search, page, pageSize, ct);

        // 🔍 Obtém uma substância específica
        public Task<Substancia?> GetAsync(int id, CancellationToken ct)
            => _repo.GetByIdAsync(id, ct);

        // ➕ Criação de nova substância
        public async Task<Substancia> CreateAsync(SubstanciaCreateDto dto, CancellationToken ct)
        {
            // Código único
            if (await _repo.GetByCodigoAsync(dto.codigo, ct) is not null)
                throw new InvalidOperationException("Código já existe.");

            // Categoria válida
            var categoria = await _ctx.Categorias.FindAsync([dto.categoriaId], ct)
                           ?? throw new InvalidOperationException("Categoria inválida.");

            // Cria a instância
            var substancia = new Substancia
            {
                Codigo = dto.codigo,
                Nome = dto.nome,
                Descricao = dto.descricao,
                Notas = dto.notas,
                CategoriaId = dto.categoriaId
            };

            // Monta as propriedades (cada propriedade tem seu tipo de valor)
            var props = await _ctx.Propriedades
                .Where(p => dto.propriedades.Select(x => x.propriedadeId).Contains(p.Id))
                .ToListAsync(ct);

            foreach (var propriedadeValorDto in dto.propriedades)
            {
                var propriedade = props.First(x => x.Id == propriedadeValorDto.propriedadeId);
                var item = new SubstanciaPropriedade
                {
                    Substancia = substancia,
                    Propriedade = propriedade,
                    ValueType = propriedadeValorDto.valueType,
                    BoolValue = propriedadeValorDto.valueType == PropertyValueType.Boolean ? propriedadeValorDto.boolValue : null,
                    DecimalValue = propriedadeValorDto.valueType == PropertyValueType.Decimal ? propriedadeValorDto.decimalValue : null
                };
                substancia.SubstanciaPropriedades.Add(item);
            }

            await _repo.AddAsync(substancia, ct);
            await _repo.SaveAsync(ct);
            return substancia;
        }

        // ✏️ Atualização de uma substância
        public async Task<Substancia?> UpdateAsync(int id, SubstanciaUpdateDto dto, CancellationToken ct)
        {
            var s = await _repo.GetByIdAsync(id, ct);
            if (s is null) return null;

            // Atualiza campos básicos (criptografados via ValueConverter)
            s.Nome = dto.nome;
            s.Descricao = dto.descricao;
            s.Notas = dto.notas;
            s.CategoriaId = dto.categoriaId;

            // Remove propriedades antigas e recria
            _ctx.SubstanciaPropriedades.RemoveRange(s.SubstanciaPropriedades);

            var props = await _ctx.Propriedades
                .Where(p => dto.propriedades.Select(x => x.propriedadeId).Contains(p.Id))
                .ToListAsync(ct);

            s.SubstanciaPropriedades = dto.propriedades.Select(pv =>
            {
                var p = props.First(x => x.Id == pv.propriedadeId);
                return new SubstanciaPropriedade
                {
                    SubstanciaId = s.Id,
                    PropriedadeId = p.Id,
                    ValueType = pv.valueType,
                    BoolValue = pv.valueType == PropertyValueType.Boolean ? pv.boolValue : null,
                    DecimalValue = pv.valueType == PropertyValueType.Decimal ? pv.decimalValue : null
                };
            }).ToList();

            await _repo.UpdateAsync(s, ct);
            await _repo.SaveAsync(ct);
            return s;
        }

        // ❌ Exclusão de substância
        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var s = await _repo.GetByIdAsync(id, ct);
            if (s is null) return false;

            await _repo.DeleteAsync(s, ct);
            await _repo.SaveAsync(ct);
            return true;
        }
    }
}
