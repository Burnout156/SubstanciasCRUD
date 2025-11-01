using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasLibrary.Dtos
{
    public record SubstanciaListItemDto(
        int id,
        string codigo,
        string nome,
        string? descricao,
        string? notas,
        int categoriaId,
        string categoriaNome
    );
}
