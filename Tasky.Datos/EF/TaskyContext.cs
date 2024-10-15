using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Tasky.Datos.EF
{
    public partial class TaskyContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
    {
        public TaskyContext(DbContextOptions<TaskyContext> options)
            : base(options)
        {
        }

        public TaskyContext() : base(new DbContextOptions<TaskyContext>())
        {
        }

        public virtual DbSet<Actividad> Actividades { get; set; } = null!;
        public virtual DbSet<Categoria> Categorias { get; set; } = null!;
        public virtual DbSet<Perfil> Perfiles { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>().ToTable("Usuario");

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("Categoria");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.ToTable("Perfil");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(250)
                    .HasColumnName("descripcion");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");
                entity.Property(e => e.IdPerfil).HasColumnName("idPerfil");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");
               

                entity.HasOne(d => d.IdPerfilNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdPerfil)
                    .HasConstraintName("FK_Usuario_Perfil");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}
