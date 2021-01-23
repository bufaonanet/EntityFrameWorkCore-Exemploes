using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    interface IProdutoDAO
    {
        void Adicionar(Produto p);
        void Remover(Produto p);
        void Atualizar(Produto p);
        List<Produto> Listar();
    }
}
