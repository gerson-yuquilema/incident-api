using Microsoft.EntityFrameworkCore;
using IncidentApi.Models;

namespace IncidentApi.Data
{
    public class IncidentDbContext : DbContext
    {
        // El constructor recibe las opciones (como la cadena de conexión) desde la inyección de dependencias
        public IncidentDbContext(DbContextOptions<IncidentDbContext> options) : base(options)
        {
        }

        // Representa la tabla en la base de datos
        public DbSet<Incident> Incidents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Le decimos explícitamente a EF Core cómo se llama la tabla
            modelBuilder.Entity<Incident>().ToTable("Incidents");
        }
    }
}