using Microsoft.EntityFrameworkCore;
using SecchiServicios.Core.Models;

namespace SecchiServicios.Data.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<ProductoServicio> ProductosServicios { get; set; }
    public DbSet<Localidad> Localidades { get; set; }
    public DbSet<Direccion> Direcciones { get; set; }
    
    public string DbPath { get; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        DbPath = Helpers.AppPaths.GetDataFilePath("SecchiServiciosDB.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.OwnsOne(c => c.Direccion, direccion =>
            {
                direccion.Property(d => d.Calle)
                    .HasColumnName("Calle")
                    .HasMaxLength(200);

                direccion.Property(d => d.Altura)
                    .HasColumnName("Altura");

                direccion.Property(d => d.Provincia)
                    .HasColumnName("Provincia");
            });
        });
    }
}