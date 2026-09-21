using Microsoft.EntityFrameworkCore;
using TallerManager.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TallerManager.Infrastructure.Data;

public class TallerManagerDbContext : IdentityDbContext<ApplicationUser>
{
    public TallerManagerDbContext(DbContextOptions<TallerManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Taller> Talleres => Set<Taller>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<OrdenServicio> Ordenes => Set<OrdenServicio>();
    public DbSet<OrdenDetalle> OrdenDetalles => Set<OrdenDetalle>();
    public DbSet<Refaccion> Refacciones => Set<Refaccion>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<FotografiaOrden> FotografiasOrden => Set<FotografiaOrden>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<Personal> Personal => Set<Personal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Taller>(entity =>
        {
            entity.HasIndex(t => t.Nombre);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasOne(c => c.Taller)
                  .WithMany(t => t.Clientes)
                  .HasForeignKey(c => c.TallerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(c => c.TallerId);
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasOne(v => v.Cliente)
                  .WithMany(c => c.Vehiculos)
                  .HasForeignKey(v => v.ClienteId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(v => v.TallerId);
            entity.HasIndex(v => v.ClienteId);
            entity.HasIndex(v => v.Placas);
            entity.HasIndex(v => v.Vin);
        });

        modelBuilder.Entity<OrdenServicio>(entity =>
        {
            entity.Property(o => o.Subtotal).HasPrecision(12, 2);
            entity.Property(o => o.Descuento).HasPrecision(12, 2);
            entity.Property(o => o.Impuestos).HasPrecision(12, 2);
            entity.Property(o => o.Total).HasPrecision(12, 2);

            entity.HasIndex(o => o.Folio).IsUnique();
            entity.HasIndex(o => o.TallerId);
            entity.HasIndex(o => o.Estado);

            entity.HasOne(o => o.Cliente)
                  .WithMany()
                  .HasForeignKey(o => o.ClienteId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Vehiculo)
                  .WithMany()
                  .HasForeignKey(o => o.VehiculoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrdenDetalle>(entity =>
        {
            entity.Property(d => d.CostoUnitario).HasPrecision(12, 2);
            entity.Property(d => d.PrecioUnitario).HasPrecision(12, 2);
            entity.Property(d => d.Subtotal).HasPrecision(12, 2);
            entity.Property(d => d.Cantidad).HasPrecision(10, 2);

            entity.HasOne(d => d.Orden)
                  .WithMany(o => o.Detalles)
                  .HasForeignKey(d => d.OrdenId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Refaccion)
                  .WithMany()
                  .HasForeignKey(d => d.RefaccionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Refaccion>(entity =>
        {
            entity.Property(r => r.Costo).HasPrecision(12, 2);
            entity.Property(r => r.Precio).HasPrecision(12, 2);

            entity.HasIndex(r => r.TallerId);
            entity.HasIndex(r => r.Codigo);
            entity.HasIndex(r => r.Sku);
        });

        modelBuilder.Entity<MovimientoInventario>(entity =>
        {
            entity.Property(m => m.CostoUnitario).HasPrecision(12, 2);

            entity.HasIndex(m => m.RefaccionId);
            entity.HasIndex(m => m.TallerId);

            entity.HasOne(m => m.Refaccion)
                  .WithMany(r => r.Movimientos)
                  .HasForeignKey(m => m.RefaccionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FotografiaOrden>(entity =>
{
    entity.HasIndex(f => f.OrdenId);
    entity.HasIndex(f => f.TallerId);

    entity.HasOne(f => f.Orden)
          .WithMany()
          .HasForeignKey(f => f.OrdenId)
          .OnDelete(DeleteBehavior.Cascade);
});

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.Property(p => p.Monto).HasPrecision(12, 2);

            entity.HasIndex(p => p.OrdenId);
            entity.HasIndex(p => p.TallerId);

            entity.HasOne(p => p.Orden)
                  .WithMany()
                  .HasForeignKey(p => p.OrdenId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Cita>(entity =>
{
    entity.HasIndex(c => c.TallerId);
    entity.HasIndex(c => c.FechaHora);

    entity.HasOne(c => c.Cliente)
          .WithMany()
          .HasForeignKey(c => c.ClienteId)
          .OnDelete(DeleteBehavior.SetNull);

    entity.HasOne(c => c.Vehiculo)
          .WithMany()
          .HasForeignKey(c => c.VehiculoId)
          .OnDelete(DeleteBehavior.SetNull);

    entity.HasOne(c => c.Orden)
          .WithMany()
          .HasForeignKey(c => c.OrdenId)
          .OnDelete(DeleteBehavior.SetNull);
});

modelBuilder.Entity<Personal>(entity =>
{
    entity.HasIndex(p => p.TallerId);

    entity.HasOne(p => p.Usuario)
          .WithMany()
          .HasForeignKey(p => p.UsuarioId)
          .OnDelete(DeleteBehavior.SetNull);
});

    }
}