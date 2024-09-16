using EntityFramework.Clases;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) {}

        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<ReporteAlumno> ReporteAlumnos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReporteAlumno>().ToTable("Reportes");
        }
    }
}
