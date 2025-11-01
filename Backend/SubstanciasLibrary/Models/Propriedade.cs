
using SubstanciasLibrary.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasLibrary.Models
{
    public class Propriedade
    {
        public int Id { get; set; }
        public string Nome { get; set; } = default!;
        // Define o tipo de valor aceito para a propriedade
        public PropertyValueType ValueType { get; set; }
        public ICollection<SubstanciaPropriedade> SubstanciaPropriedades { get; set; } = new List<SubstanciaPropriedade>();
    }
}
