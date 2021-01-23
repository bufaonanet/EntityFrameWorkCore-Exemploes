using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alura.Filmes.App.Negocio
{
    public class FilmeCategoria
    {
        public int FilmeId { get; set; }
        public Filme Filme { get; set; }
        public byte CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

    }
}
