
namespace Entities
{
    public class Compra
    {
        public int Id { get; set; }
        public int Quantidade { get; internal set; }
        public double Preco { get; internal set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }
        
        public Compra()
        {
        }

        public override string ToString()
        {
            return $"Compra:{Id}, qtd:{Quantidade}, Total:{Preco}, produto:{Produto.Nome}";
        }
    }
}