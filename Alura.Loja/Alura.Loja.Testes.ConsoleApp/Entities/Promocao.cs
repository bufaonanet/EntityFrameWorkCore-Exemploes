using System;
using System.Collections.Generic;

namespace Entities
{
    public class Promocao
    {
        public int Id { get; set; }
        public string Descricao { get; internal set; }
        public DateTime DataInicio { get; internal set; }
        public DateTime DataFim { get; internal set; }
        public IList<PromocaoProduto> Produtos { get; }

        public Promocao()
        {
            Produtos = new List<PromocaoProduto>();
        }

        public void AdicionaProduto(Produto produto)
        {
            Produtos.Add(new PromocaoProduto() { Produto = produto });
        }
    }
}
