using Microsoft.EntityFrameworkCore;
using SubstanciasDatabase.Criptografia;
using SubstanciasLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasDatabase
{
    public class AppDbContext : DbContext
    {
        private readonly AesGcmStringProtector _protector;

        public AppDbContext(DbContextOptions<AppDbContext> options, AesGcmStringProtector protector) : base(options)
        {
            _protector = protector;
        }

        public DbSet<Substancia> Substancias => Set<Substancia>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Propriedade> Propriedades => Set<Propriedade>();
        public DbSet<SubstanciaPropriedade> SubstanciaPropriedades => Set<SubstanciaPropriedade>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.HasPostgresExtension("uuid-ossp");

            // Categoria
            mb.Entity<Categoria>(e =>
            {
                e.ToTable("categorias");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Nome).HasMaxLength(150).IsRequired();
                e.HasIndex(x => x.Nome).IsUnique();
            });

            // Propriedade
            mb.Entity<Propriedade>(e =>
            {
                e.ToTable("propriedades");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Nome).HasMaxLength(150).IsRequired();
                e.HasIndex(x => x.Nome).IsUnique();
                e.Property(x => x.ValueType).HasConversion<int>();
            });

            // Substancia (com criptografia em Nome/Descricao/Notas)
            var enc = new EncryptedStringConverter(_protector);

            mb.Entity<Substancia>(e =>
            {
                e.ToTable("substancias");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Codigo).HasMaxLength(60).IsRequired();
                e.HasIndex(x => x.Codigo).IsUnique();

                e.Property(x => x.Nome).HasMaxLength(200).IsRequired().HasConversion(enc);
                e.Property(x => x.Descricao).HasConversion(enc);
                e.Property(x => x.Notas).HasConversion(enc);

                e.HasOne(x => x.Categoria)
                    .WithMany(c => c.Substancias)
                    .HasForeignKey(x => x.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SubstanciaPropriedade
            mb.Entity<SubstanciaPropriedade>(e =>
            {
                e.ToTable("substancia_propriedades");
                e.HasKey(x => new { x.SubstanciaId, x.PropriedadeId });

                e.HasOne(sp => sp.Substancia)
                    .WithMany(s => s.SubstanciaPropriedades)
                    .HasForeignKey(sp => sp.SubstanciaId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(sp => sp.Propriedade)
                    .WithMany(p => p.SubstanciaPropriedades)
                    .HasForeignKey(sp => sp.PropriedadeId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.ValueType).HasConversion<int>();
                e.Property(x => x.DecimalValue).HasColumnType("numeric(18,4)");
            });

            base.OnModelCreating(mb);
        }
    }
}
