using Batch.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Batch.Data
{
    public class AppDbContext : IdentityDbContext<Usuario>

    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Aquí defines tus tablas
        public DbSet<Componente> Componentes { get; set; }
        public DbSet<Tolerancia> Tolerancias { get; set; }
        public DbSet<Lote> Batches { get; set; }
        public DbSet<ResultadoPrueba> ResultadosPrueba { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------------
            // Componente
            // -------------------------------
            modelBuilder.Entity<Componente>(entity =>
            {
                entity.ToTable("Componentes");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.BatchCar)
                      .HasMaxLength(50);

                entity.HasMany(e => e.Tolerancias)
                      .WithOne(t => t.Componente)
                      .HasForeignKey(t => t.ComponenteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -------------------------------
            // Tolerancia
            // -------------------------------
            modelBuilder.Entity<Tolerancia>(entity =>
            {
                entity.ToTable("Tolerancias");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Prueba)
                      .HasColumnType("nvarchar(100)")
                      .IsRequired();

                entity.Property(e => e.Max).HasColumnType("float");
                entity.Property(e => e.Min).HasColumnType("float");
            });

            // -------------------------------
            // Lote
            // -------------------------------
            modelBuilder.Entity<Lote>(entity =>
            {
                entity.ToTable("Batches");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Folio)
                      .HasMaxLength(30)
                      .IsRequired();

                entity.HasIndex(e => e.Folio);

                entity.Property(e => e.RegistroId)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasIndex(e => e.RegistroId)
                      .IsUnique();

                entity.HasOne(e => e.Componente)
                      .WithMany()
                      .HasForeignKey(e => e.ComponenteId)
                      .OnDelete(DeleteBehavior.Cascade);

                // ✅ Relación corregida
                entity.HasMany(e => e.Resultados)
                      .WithOne(r => r.Lote)   // ← AQUÍ ESTABA EL ERROR
                      .HasForeignKey(r => r.LoteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -------------------------------
            // ResultadoPrueba
            // -------------------------------
            modelBuilder.Entity<ResultadoPrueba>(entity =>
            {
                entity.ToTable("ResultadosPrueba");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Valor).HasColumnType("float");

                entity.HasOne(r => r.Tolerancia)
                      .WithMany()
                      .HasForeignKey(r => r.ToleranciaId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

          


        }



    }

}
