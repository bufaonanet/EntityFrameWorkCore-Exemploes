using Alura.Filmes.App.Dados;
using Alura.Filmes.App.Extensions;
using Alura.Filmes.App.Negocio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Alura.Filmes.App
{
    class Program
    {
        public static void ImprimeFilmes()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                contexto.LogSQLToConsole();

                var atores = contexto.Filmes;

                foreach (var filme in atores)
                {
                    Console.WriteLine(filme);
                }
            }

        }
        public static void ImprimeAtores()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                contexto.LogSQLToConsole();

                var atores = contexto.Atores
                    .OrderBy(a => EF.Property<DateTime>(a, "last_update"))
                    .Take(10);

                foreach (var ator in atores)
                {
                    Console.WriteLine(ator + " - " + contexto.Entry(ator).Property("last_update").CurrentValue);
                }
            }

        }
        public static void ImprimeFilmeAtores()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                contexto.LogSQLToConsole();

                var filme = contexto
                    .Filmes
                    .Include(f => f.Elenco)
                    .ThenInclude(fa => fa.Ator)
                    .First();

                Console.WriteLine(filme);
                Console.WriteLine("Elenco");

                foreach (var ator in filme.Elenco)
                {
                    Console.WriteLine(ator.Ator);
                }
            }
        }
        public static void ImprimeIdiomaEFilmes()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                contexto.LogSQLToConsole();

                var idiomas = contexto
                    .Idiomas
                    .Include(i => i.FilmesFalados);

                foreach (var idioma in idiomas)
                {
                    Console.WriteLine(idioma);

                    foreach (var filme in idioma.FilmesFalados)
                    {
                        Console.WriteLine(filme);
                    }

                    Console.WriteLine("\n ------------------------------ \n");
                }
            }
        }
        public static void ImprimeCategoriasComFilmes()
        {
            using (var contexto = new AluraFilmesContexto())
            {
                contexto.LogSQLToConsole();

                var categorias = contexto.Categorias
                    .Include(c => c.Filmes)
                    .ThenInclude(fc => fc.Filme);

                foreach (var c in categorias)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"Filmes da categoria {c}");

                    foreach (var fc in c.Filmes)
                    {
                        Console.WriteLine(fc.Filme);
                    }
                }
            }
        }
        public static void AdicionandoFilmeEMostrando()
        {
            using (var context = new AluraFilmesContexto())
            {
                context.LogSQLToConsole();

                var filme = new Filme
                {
                    Titulo = "Teste filme",
                    Duracao = 120,
                    AnoLancamento = "2020",
                    Classificacao = ClassificacaoIndicativa.MaioresQue18,
                    IdiomaFalado = context.Idiomas.First()
                };

                context.Filmes.Add(filme);
                context.SaveChanges();

                var filmeInserido = context.Filmes.First(f => f.Titulo == "Teste filme");
                Console.WriteLine(filmeInserido);

            }
        }
        public static void ImprimindoClientesFuncionarios(AluraFilmesContexto context)
        {
            var clientes = context.Clientes;

            Console.WriteLine("\n--- Clientes ---");
            foreach (var c in clientes)
            {
                Console.WriteLine(c);

            }

            var funcionarios = context.Funcionarios;

            Console.WriteLine("\n--- Funcionários ---");
            foreach (var f in funcionarios)
            {
                Console.WriteLine(f);

            }
        }
        public static void ExecutandoSqlCustomizado(AluraFilmesContexto context)
        {
            var sql = @"
                    select a.*
                        from actor a 
                        join top5_most_starred_actors filmes on filmes.actor_id = a.actor_id
                        ";

            var atores = context
                .Atores
                .FromSql(sql)
                .Include(a => a.Filmografia)
                .OrderByDescending(a => a.Filmografia.Count);

            foreach (var a in atores)
            {
                Console.WriteLine($"{a.PrimeiroNome + ' ' + a.UltimoNome} - Filmes:{a.Filmografia.Count} ");

            }


        }
        public static void ExecutaStoreProcedure(DbContext context)
        {
            var categ = "Action";

            var paramCateg = new SqlParameter("category_name", categ);
            var paramTotal = new SqlParameter
            {
                ParameterName = "total_actors",
                Size = 4,
                Direction = System.Data.ParameterDirection.Output
            };

            context.Database
                .ExecuteSqlCommand(
                    "execute total_actors_from_given_category @category_name, @total_actors out",
                     paramCateg,
                     paramTotal
                 );

            Console.WriteLine($"Total de atores que atuaram na categorias {categ} é de {paramTotal.Value}");
        }


        static void Main()
        {
            using (var context = new AluraFilmesContexto())
            {
                context.LogSQLToConsole();

                var sql = "INSERT INTO language (name) VALUES ('Teste1'),('Teste2'),('Teste3')";

                var registros = context.Database.ExecuteSqlCommand(sql);
                Console.WriteLine($"Total de ({registros}) registros inseridos");
                
                sql = "DELETE FROM language WHERE name like 'Teste%'";

                registros = context.Database.ExecuteSqlCommand(sql);
                Console.WriteLine($"Total de ({registros}) registros excluidos");

            }
        }
    }
}