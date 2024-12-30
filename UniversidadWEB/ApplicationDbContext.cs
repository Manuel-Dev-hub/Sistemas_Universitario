using Microsoft.EntityFrameworkCore;
using UniversidadWEB.Models;
using UniversidadWEB;

namespace UniversidadWEB
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Catedratico> Catedraticos { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Asignacion> Asignaciones { get; set; }
        public DbSet<Nota> Notas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones
            modelBuilder.Entity<Curso>()
                .HasOne(c => c.Carrera)
                .WithMany()
                .HasForeignKey(c => c.CarreraID);

            modelBuilder.Entity<Asignacion>()
                .HasOne(a => a.Alumno)
                .WithMany()
                .HasForeignKey(a => a.AlumnoID);

            modelBuilder.Entity<Asignacion>()
                .HasOne(a => a.Curso)
                .WithMany()
                .HasForeignKey(a => a.CursoID);

            modelBuilder.Entity<Asignacion>()
                .HasOne(a => a.Catedratico)
                .WithMany()
                .HasForeignKey(a => a.CatedraticoID);

            modelBuilder.Entity<Nota>()
    .HasOne(n => n.Asignacion)
    .WithMany()
    .HasForeignKey(n => n.AsignacionID)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Nota>()
     .Property(n => n.Calificacion)
     .HasColumnType("decimal(5,2)");
        }
    }
} 