using Batch.Models;
using Microsoft.EntityFrameworkCore;


namespace Batch.Data
{
    public class AppDbContext : DbContext
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

            modelBuilder.Entity<Componente>(entity =>
            {
                entity.ToTable("Componentes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BatchCar).HasMaxLength(50);
            });

            modelBuilder.Entity<Tolerancia>(entity =>
            {
                entity.ToTable("Tolerancias");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Prueba)
                      .HasColumnType("nvarchar(100)")
                      .IsRequired();

                entity.Property(e => e.Max).HasColumnType("float");
                entity.Property(e => e.Min).HasColumnType("float");

                entity.HasOne(e => e.Componente)
                      .WithMany() // o .WithMany(c => c.Tolerancias) si tienes colección
                      .HasForeignKey(e => e.ComponenteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Lote>(entity =>
            {
                entity.ToTable("Batches"); // Tabla sigue llamándose Batches
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Folio).HasMaxLength(30).IsRequired();
                entity.HasIndex(e => e.Folio).IsUnique();
                entity.HasOne(e => e.Componente)
                      .WithMany()
                      .HasForeignKey(e => e.ComponenteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ResultadoPrueba>(entity =>
            {
                entity.ToTable("ResultadosPrueba");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Batch)
                      .WithMany(b => b.Resultados)
                      .HasForeignKey(e => e.BatchId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Tolerancia)
                      .WithMany()
                      .HasForeignKey(e => e.ToleranciaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


        }



    }

}
