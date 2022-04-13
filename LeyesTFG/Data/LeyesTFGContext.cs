#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LeyesTFG.Models;

namespace LeyesTFG.Data
{
    public class LeyesTFGContext : DbContext
    {
        public LeyesTFGContext (DbContextOptions<LeyesTFGContext> options)
            : base(options)
        {
        }

        public DbSet<Articulo> Articulo { get; set; }

        public DbSet<Modificacion> Modificacion { get; set; }

        public DbSet<Ley> Ley { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ley>()
                .HasMany(c => c.Articulos)
                .WithOne(e => e.Ley)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Articulo>()
                .HasMany(f => f.Modificaciones)
                .WithOne(g => g.Articulo)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
