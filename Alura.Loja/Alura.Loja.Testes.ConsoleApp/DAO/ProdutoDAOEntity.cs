using Context;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DAO
{
    class ProdutoDAOEntity : IProdutoDAO, IDisposable
    {
        private LojaContext _context;

        public ProdutoDAOEntity()
        {
            _context = new LojaContext();
        }

        public void Adicionar(Produto p)
        {
            _context.Produtos.Add(p);
            _context.SaveChanges();
        }

        public void Atualizar(Produto p)
        {
            _context.Produtos.Update(p);
            _context.SaveChanges();
        }

        public List<Produto> Listar()
        {
            return _context.Produtos.ToList();
        }

        public void Remover(Produto p)
        {
            _context.Produtos.Remove(p);
            _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
