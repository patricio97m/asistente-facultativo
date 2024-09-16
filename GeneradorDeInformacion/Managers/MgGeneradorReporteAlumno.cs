using Entidades;
using GeneradorDeInformacion.Managers.Interfaces;
using System.Diagnostics;

namespace GeneradorDeInformacion.Managers
{
    internal class MgGeneradorReporteAlumno : IMgGenerador
    {
        private int cantRegistros;
        List<ReporteAlumno> reportes = new List<ReporteAlumno>();

        public MgGeneradorReporteAlumno(int cantidad)
        {
            cantRegistros = cantidad;
        }

        public void Generar()
        {
            Random random = new Random();

            Dictionary<int, Materia> materias = GenerarDictionaryMaterias();

            for (int i = 0; i < cantRegistros; i += 1)
            {
                ReporteAlumno reporte = new ReporteAlumno();

                reporte.Alumno = new Alumno();
                reporte.Alumno.Id = random.Next(1, 5000);

                int idMateria = random.Next(1, 5);
                reporte.Materia = materias[idMateria];
                reporte.Materia.Id = idMateria;

                reporte.NotaFinal = random.Next(1, 11);
                reporte.Promocionable = reporte.NotaFinal >= 7;

                reportes.Add(reporte);
            }
        }

        private Dictionary<int, Materia> GenerarDictionaryMaterias()
        {
            return new Dictionary<int, Materia>()
            {
                { 2619, new Materia("PROGRAMACION BASICA I") },
                { 2620, new Materia("INFORMATICA GENERAL") },
                { 2621, new Materia("MATEMATICA GENERAL") },
                { 2622, new Materia("INGLES TECNICO I") },
                { 2623, new Materia("PROGRAMACION BASICA II") },
                { 2624, new Materia("PROGRAMACION WEB I") },
                { 2625, new Materia("BASES DE DATOS I") },
                { 2626, new Materia("INTRODUCCION AL DISEÑO GRAFICO EN LA WEB") },
                { 2627, new Materia("INGLES TECNICO II") },
                { 2628, new Materia("PROGRAMACION WEB II") },
                { 2629, new Materia("DISEÑO DE APLICACIONES WEB") },
                { 2630, new Materia("VISUALIZACION E INTERFACES") },
                { 2631, new Materia("TALLER WEB I") },
                { 2632, new Materia("BASE DE DATOS II") },
                { 2633, new Materia("PROGRAMACION WEB III") },
                { 2634, new Materia("TECNOLOGIA DE REDES") },
                { 2635, new Materia("TALLER WEB II") },
                { 2636, new Materia("SEGURIDAD Y CALIDAD EN APLICACION WEB") },
                { 2637, new Materia("INTRODUCCION A LA ADMINISTRACION DE PROYECTOS") },
                { 2638, new Materia("TALLER PRACTICO INTEGRADOR") }
            };
        }

        public void CrearCsv()
        {
            string directorioProyecto = Directory.GetCurrentDirectory();
            string directorioInformacion = Path.Combine(directorioProyecto, "data");
            Directory.CreateDirectory(directorioInformacion);

            string carpetaArchivo = Path.Combine(directorioInformacion, "reporte.csv");
            using (StreamWriter writer = new StreamWriter(carpetaArchivo))
            {
                writer.WriteLine("IdAlumno, IdMateria,NotaPromocionFinal,Promocionable");

                foreach (ReporteAlumno reporte in reportes)
                {
                    writer.WriteLine($"{reporte.Alumno.Id},{reporte.Materia.Id},{reporte.NotaFinal},{Convert.ToInt32(reporte.Promocionable)}");
                }
            }
            Process.Start("explorer.exe", directorioInformacion);
        }
    }
}

