using Alura.Filmes.App.Negocio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alura.Filmes.App.Dados
{
    class FilmeCategoriaConfiguration : IEntityTypeConfiguration<FilmeCategoria>
    {
        public void Configure(EntityTypeBuilder<FilmeCategoria> builder)
        {
            builder.ToTable("film_category");

            builder
                .Property(fc => fc.CategoriaId)
                .HasColumnName("category_id");

            builder
                .Property(fc => fc.FilmeId)
                .HasColumnName("film_id");

            builder
                .HasKey(fc => new { fc.FilmeId, fc.CategoriaId });

            builder
              .Property<DateTime>("last_update")
              .HasColumnType("datetime")
              .HasDefaultValueSql("getdate()");

            builder
                .HasOne(fc => fc.Filme)
                .WithMany(f => f.Categorias)
                .HasForeignKey(fc => fc.FilmeId); 
            
            builder
                .HasOne(fc => fc.Categoria)
                .WithMany(c => c.Filmes)
                .HasForeignKey(fc => fc.CategoriaId);

        }
    }
}
