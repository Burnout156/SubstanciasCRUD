using SubstanciasLibrary.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasLibrary.Dtos
{
    public record PropriedadeValorDto
        (int propriedadeId, PropertyValueType valueType, bool? boolValue, decimal? decimalValue)
    ;
}
