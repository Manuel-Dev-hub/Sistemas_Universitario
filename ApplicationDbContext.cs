using Microsoft.EntityFrameworkCore;
using UniversidadWEB.Models;

namespace UniversidadWEB.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Alumno> Alumnos { get; set; }
    }
}