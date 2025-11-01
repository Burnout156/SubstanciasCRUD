using SubstanciasLibrary.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasLibrary.Models
{
    public class SubstanciaPropriedade
    {
        public int SubstanciaId { get; set; }
        public Substancia Substancia { get; set; } = default!;

        public int PropriedadeId { get; set; }
        public Propriedade Propriedade { get; set; } = default!;

        // Valor com 2 colunas opcionais + enum de tipo
        public PropertyValueType ValueType { get; set; }
        public bool? BoolValue { get; set; }
        public decimal? DecimalValue { get; set; }
    }
}
