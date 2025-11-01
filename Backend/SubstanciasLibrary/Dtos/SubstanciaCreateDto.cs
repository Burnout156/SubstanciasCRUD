using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasLibrary.Dtos
{
    public record SubstanciaCreateDto(
       string codigo,
       string nome,
       string? descricao,
       string? notas,
       int categoriaId,
       List<PropriedadeValorDto> propriedades
    );
}
