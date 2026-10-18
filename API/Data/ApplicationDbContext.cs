using Microsoft.EntityFrameworkCore;
using UsuariosApi.Entities;

namespace UsuariosApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");

            e.HasKey(x => x.Id);

            e.Property(x => x.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.Apellido)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.Correo)
                .HasMaxLength(150)
                .IsRequired();

            e.Property(x => x.FechaNacimiento).HasColumnType("date").IsRequired();
            e.Property(x => x.Universidad).HasMaxLength(150).IsRequired();
            e.Property(x => x.Semestre).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
            e.ToTable(t => t.HasCheckConstraint("CK_Usuarios_Semestre", "`Semestre` BETWEEN 1 AND 10"));

            e.HasIndex(x => x.Correo)
                .IsUnique();
        });
    }
}