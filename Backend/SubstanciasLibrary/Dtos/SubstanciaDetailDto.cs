
using SubstanciasLibrary.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasLibrary.Dtos
{
    public record SubstanciaDetailDto(
        int id,
        string codigo,
        string nome,
        string? descricao,
        string? notas,
        int categoriaId,
        string categoriaNome,
        List<(int propriedadeId, string propriedadeNome, PropertyValueType valueType, 
              bool? boolValue, decimal? decimalValue)> propriedades
    );
}
