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
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }
        public DbSet<RolPermiso> RolPermisos { get; set; }

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

            // UsuarioRol
            modelBuilder.Entity<UsuarioRol>()
                .HasKey(ur => new { ur.UsuarioId, ur.RolId });

            modelBuilder.Entity<UsuarioRol>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.UsuarioRoles)
                .HasForeignKey(ur => ur.UsuarioId);

            modelBuilder.Entity<UsuarioRol>()
                .HasOne(ur => ur.Rol)
                .WithMany(r => r.UsuarioRoles)
                .HasForeignKey(ur => ur.RolId);

            // RolPermiso
            modelBuilder.Entity<RolPermiso>()
                .HasKey(rp => new { rp.RolId, rp.PermisoId });

            modelBuilder.Entity<RolPermiso>()
                .HasOne(rp => rp.Rol)
                .WithMany(r => r.RolPermisos)
                .HasForeignKey(rp => rp.RolId);

            modelBuilder.Entity<RolPermiso>()
                .HasOne(rp => rp.Permiso)
                .WithMany(p => p.RolPermisos)
                .HasForeignKey(rp => rp.PermisoId);


        }



    }

}
