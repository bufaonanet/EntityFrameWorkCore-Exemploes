using System;
using System.Collections.Generic;
using System.Linq;
using DAO;
using Context;
using Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Alura.Loja.Testes.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var contexto = new LojaContext())
            {
                LogSqlConsole(contexto);

                var cliente = contexto
                    .Clientes
                    .Include(c => c.EnderecoDeEntrega)
                    .FirstOrDefault();

                Console.WriteLine($"Endereço de entrega de {cliente.Nome} é:\n {cliente.EnderecoDeEntrega}");

                var produto = contexto
                    .Produtos
                    .Where(p => p.Id == 3002)
                    .FirstOrDefault();

                contexto.Entry(produto)
                    .Collection(p => p.Compras)
                    .Query()
                    .Where(c => c.Preco < 20.00)
                    .Load();


                Console.WriteLine($"Mostrando as compras do produto:{produto.Nome}");
                foreach (var item in produto.Compras)
                {
                    Console.WriteLine(item);
                }
            }
        }

        private static void ExibeProdutosDaPromocao()
        {
            using (var contexto = new LojaContext())
            {
                LogSqlConsole(contexto);

                var promocao = contexto.Promocoes
                    .Include(p => p.Produtos)
                    .ThenInclude(pp => pp.Produto)
                    .FirstOrDefault();

                Console.WriteLine("\nProdutos da promoção:" + promocao.Descricao);
                foreach (var item in promocao.Produtos)
                {
                    Console.WriteLine(item.Produto);
                }

            }
        }

        private static void InserirPromocao()
        {
            using (var contexto = new LojaContext())
            {
                LogSqlConsole(contexto);

                var promocao = new Promocao()
                {
                    Descricao = "Queimão Total Janeiro 2019",
                    DataInicio = DateTime.Now,
                    DataFim = DateTime.Now.AddMonths(1)
                };

                var listaProdutos = contexto
                    .Produtos
                    .Where(p => p.Categoria == "Bebidas")
                    .ToList();

                foreach (var item in listaProdutos)
                {
                    promocao.AdicionaProduto(item);
                }

                contexto.Promocoes.Add(promocao);

                ExibeEntries(contexto.ChangeTracker.Entries());
                contexto.SaveChanges();
            }
        }

        private static void RelacionamentoUmParaUm()
        {
            var fulano = new Cliente();
            fulano.Nome = "Fulano de Tal";
            fulano.EnderecoDeEntrega = new Endereco()
            {
                Numero = 12,
                Logradouro = "Rua dos bobos",
                Complemento = "Sobrado",
                Bairro = "Centro",
                Cidade = "Guanhães"
            };

            using (var contexto = new LojaContext())
            {
                //mostrar os logs de operação com o entity no console
                LogSqlConsole(contexto);

                contexto.Clientes.Add(fulano);
                contexto.SaveChanges();

            }
        }
        static void RelacionamentoUmParaMuitos()
        {
            var p1 = new Produto() { Nome = "Suco de Laranja", Categoria = "Bebidas", PrecoUnitario = 8.79, Unidade = "Litros" };
            var p2 = new Produto() { Nome = "Café", Categoria = "Bebidas", PrecoUnitario = 12.45, Unidade = "Gramas" };
            var p3 = new Produto() { Nome = "Macarrão", Categoria = "Alimentos", PrecoUnitario = 4.23, Unidade = "Gramas" };

            var promocao = new Promocao();
            promocao.Descricao = "Promocao de Páscoa";
            promocao.DataInicio = DateTime.Now;
            promocao.DataFim = DateTime.Now.AddMonths(3);

            promocao.AdicionaProduto(p1);
            promocao.AdicionaProduto(p2);
            promocao.AdicionaProduto(p3);


            using (var contexto = new LojaContext())
            {
                LogSqlConsole(contexto);

                //  var promocao = contexto.Promocoes.Find(1);

                contexto.Remove(promocao);

                ExibeEntries(contexto.ChangeTracker.Entries());
                contexto.SaveChanges();

            }
        }
        private static void LogSqlConsole(LojaContext contexto)
        {
            var serviceProvider = contexto.GetInfrastructure<IServiceProvider>();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(SqlLoggerProvider.Create());
        }
        private static void EstadosDoEntity()
        {
            using (var contexto = new LojaContext())
            {
                var serviceProvider = contexto.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                loggerFactory.AddProvider(SqlLoggerProvider.Create());


                var produtos = contexto.Produtos.ToList();

                ExibeEntries(contexto.ChangeTracker.Entries());

                var p = new Produto()
                {
                    Nome = "Lata",
                    Categoria = "Alimentos",
                    PrecoUnitario = 11.20
                };

                contexto.Add(p);

                p.Nome = "novo";

                contexto.Remove(p);

                var entry = contexto.Entry(p);
                Console.WriteLine("\n\n " + entry.Entity.ToString() + " - " + entry.State);

                //var p = contexto.Produtos.First();
                //contexto.Remove(p);

                //ExibeEntries(contexto.ChangeTracker.Entries());

                //contexto.SaveChanges();

                //ExibeEntries(contexto.ChangeTracker.Entries());
            }
        }
        private static void ExibeProdutos(List<Produto> produtos)
        {
            foreach (var item in produtos)
            {
                Console.WriteLine(item.Nome);
            }
        }
        private static void ExibeEntries(IEnumerable<EntityEntry> entries)
        {
            Console.WriteLine("=====================");

            foreach (var e in entries)
            {
                Console.WriteLine(e.Entity.ToString() + " - " + e.State);
            }

            Console.WriteLine("=====================");
        }
        private static void AtualizarProduto()
        {
            GravarUsandoEntity();
            RecuperaProdutos();

            using (var contexto = new ProdutoDAOEntity())
            {
                Produto p = new Produto();

                p.Nome = "Produto editado";

                contexto.Adicionar(p);
            }

            RecuperaProdutos();
        }
        private static void DeletarProdutos()

        {

            using (var contexto = new ProdutoDAOEntity())
            {
                List<Produto> produtos = contexto.Listar();

                foreach (var item in produtos)
                {
                    contexto.Remover(item);
                }
            }
        }
        private static void RecuperaProdutos()
        {
            using (var contexto = new ProdutoDAOEntity())
            {
                var produtos = contexto.Listar();

                Console.WriteLine($"Foram encontrados {produtos.Count} produtos.");

                foreach (var item in produtos)
                {
                    Console.WriteLine(item.Nome);
                }
            }
        }
        private static void GravarUsandoEntity()
        {
            Produto p1 = new Produto();
            p1.Nome = "Harry Potter e a Ordem da Fênix";
            p1.Categoria = "Livros";
            p1.PrecoUnitario = 19.89;

            Produto p2 = new Produto();
            p2.Nome = "Senhor dos Anéis 1";
            p2.Categoria = "Livros";
            p2.PrecoUnitario = 19.89;

            Produto p3 = new Produto();
            p3.Nome = "O Monge e o Executivo";
            p3.Categoria = "Livros";
            p3.PrecoUnitario = 19.89;

            using (var contexto = new ProdutoDAOEntity())
            {
                contexto.Adicionar(p1);
                contexto.Adicionar(p2);
                contexto.Adicionar(p3);
            }
        }
        private static void GravarUsandoAdoNet()
        {
            Produto p = new Produto();
            p.Nome = "Harry Potter e a Ordem da Fênix";
            p.Categoria = "Livros";
            p.PrecoUnitario = 19.89;

            using (var repo = new ProdutoDAO())
            {
                repo.Adicionar(p);
            }
        }
    }
}
