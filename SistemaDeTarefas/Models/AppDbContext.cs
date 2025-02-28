using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace SistemaDeTarefas.Models
{
    class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
            "Server=Higor;Database=SistemaDeTarefas;Trusted_Connection=True;TrustServerCertificate=True;");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pessoa>()
             .HasOne(p => p.Credenciais)
             .WithOne(c => c.Pessoa)
             .HasForeignKey<Credenciais>(c => c.PessoaId);

            modelBuilder.Entity<Tarefas>()
                 .HasOne(t => t.Pessoa)
                 .WithMany(p => p.Tarefas)
                 .HasForeignKey(p => p.PessoaId);
                
        }
    }
}
