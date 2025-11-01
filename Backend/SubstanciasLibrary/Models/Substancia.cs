using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasLibrary.Models
{
    public class Substancia
    {
        public int Id { get; set; }

        // Código único para permitir índice único
        public string Codigo { get; set; } = default!;

        // Criptografados via ValueConverter
        public string Nome { get; set; } = default!;
        public string? Descricao { get; set; }
        public string? Notas { get; set; }

        // Relacionamentos
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = default!;

        public ICollection<SubstanciaPropriedade> SubstanciaPropriedades { get; set; } = new List<SubstanciaPropriedade>();
    }
}
