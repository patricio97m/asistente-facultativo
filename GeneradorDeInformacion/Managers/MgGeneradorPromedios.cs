using Entidades;
using GeneradorDeInformacion.Managers.Interfaces;
using System.Diagnostics;

namespace GeneradorDeInformacion.Managers
{
    internal class MgGeneradorPromedios : IMgGenerador
    {
        private int cantRegistros;
        Dictionary<int, ReportePrediccion> reportesPorAlumno = new Dictionary<int, ReportePrediccion>();

        public MgGeneradorPromedios(int cantidad)
        {
            cantRegistros = cantidad;
        }

        public void Generar()
        {
            Random random = new Random();
            float promedioPromocionTotalAlumnos = 0;
            int cantidadAlumnos = 0;
            Dictionary<int, Materia> materias = GenerarDictionaryMaterias();

            List<int> materiasIds = new List<int>(materias.Keys);

            HashSet<Tuple<int, int>> combinacionesGeneradas = new HashSet<Tuple<int, int>>();


            for (int i = 0; i < cantRegistros; i++)
            {
                ReportePrediccion reporte = new ReportePrediccion();
                int idAlumno;
                int idMateria;

                do
                {
                    idAlumno = random.Next(1, 10000);
                    idMateria = materiasIds[random.Next(materiasIds.Count)];
                } while (!combinacionesGeneradas.Add(new Tuple<int, int>(idAlumno, idMateria)));

                reporte.IdAlumno = idAlumno;
                reporte.IdMateria = idMateria;

                ReportePrediccion reporteAnterior = reportesPorAlumno.FirstOrDefault(x => x.Value.IdAlumno == idAlumno).Value;

                if (reporteAnterior != null)
                {
                    reporte.PromedioPromocionAlumno = reporteAnterior.PromedioPromocionAlumno;
                    reporte.CantMateriasElegidas = reporteAnterior.CantMateriasElegidas;
                }
                else
                {
                    reporte.PromedioPromocionAlumno = random.NextSingle() * 10;
                    reporte.CantMateriasElegidas = Convert.ToInt32(random.NextSingle() * 5);
                    promedioPromocionTotalAlumnos += reporte.PromedioPromocionAlumno;
                    cantidadAlumnos++;
                }

                reporteAnterior = reportesPorAlumno.FirstOrDefault(x => x.Value.IdMateria == idMateria).Value;

                if (reporteAnterior != null && reporteAnterior.IdMateria == idMateria)
                {
                    reporte.PromedioPromocionMateria = reporteAnterior.PromedioPromocionMateria;
                }
                else
                {
                    reporte.PromedioPromocionMateria = random.NextSingle() * 9 + 1;
                }

                reportesPorAlumno[i] = reporte;
            }

            float promedioPromocionOtros = promedioPromocionTotalAlumnos / cantidadAlumnos;

            foreach (var reporte in reportesPorAlumno)
            {
                reporte.Value.PromedioPromocionOtros = promedioPromocionOtros;
                reporte.Value.PrediccionPromocion = (reporte.Value.CantMateriasElegidas == 1) ||

                                                    (reporte.Value.PromedioPromocionAlumno >= 9 &&
                                                    reporte.Value.CantMateriasElegidas <= 5) ||

                                                    (reporte.Value.PromedioPromocionMateria >= 9 && reporte.Value.PromedioPromocionAlumno > 2 &&
                                                    reporte.Value.CantMateriasElegidas <= 3) ||

                                                    (reporte.Value.PromedioPromocionMateria >= 7 && reporte.Value.CantMateriasElegidas <= 2) ||

                                                    (reporte.Value.PromedioPromocionMateria >= 6 && reporte.Value.PromedioPromocionAlumno >= 4 &&
                                                    reporte.Value.CantMateriasElegidas <= 3) ||

                                                    (reporte.Value.PromedioPromocionMateria >= 5 && reporte.Value.PromedioPromocionAlumno >= 7 && 
                                                    reporte.Value.CantMateriasElegidas <= 4) ||

                                                    (reporte.Value.PromedioPromocionAlumno >= 6 && 
                                                    reporte.Value.PromedioPromocionAlumno >= reporte.Value.PromedioPromocionMateria &&
                                                    reporte.Value.CantMateriasElegidas <= 4) ||

                                                    (reporte.Value.PromedioPromocionMateria >= 4 && 
                                                    reporte.Value.PromedioPromocionAlumno >= promedioPromocionOtros &&
                                                    reporte.Value.CantMateriasElegidas <= 3)

                                                    ? 1 : 0;
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
            Random random = new Random();
          
            string directorioProyecto = Directory.GetCurrentDirectory();
            string directorioInformacion = Path.Combine(directorioProyecto, "data");
            Directory.CreateDirectory(directorioInformacion);

            string carpetaArchivo = Path.Combine(directorioInformacion, "InsertsReporteAlumno.csv");
            using (StreamWriter writer = new StreamWriter(carpetaArchivo))
            {
                writer.WriteLine("IdAlumno,PromedioPromocionAlumno,CantMateriasElegidas,PromedioPromocionOtros,IdMateria,PromedioPromocionMateria,PrediccionPromocion");

                foreach (var reporte in reportesPorAlumno.OrderBy(x => x.Value.IdAlumno))
                {
                    int nota = random.Next(1, 11);
                    int promociona = (nota >= 4) ? 1 : 0;
                    string promedioPromocionAlumnoSinComa = reporte.Value.PromedioPromocionAlumno.ToString("F1").Replace(',', '.');
                    string promedioPromocionOtrosSinComa = reporte.Value.PromedioPromocionOtros.ToString("F1").Replace(',', '.');
                    string promedioPromocionMateriaSinComa = reporte.Value.PromedioPromocionMateria.ToString("F1").Replace(',', '.');

                    writer.WriteLine($"INSERT INTO Reportes VALUES({reporte.Value.IdMateria},{reporte.Value.IdAlumno},{nota},{promociona})");
                }
            }
            Process.Start("explorer.exe", directorioInformacion);
        }
    }
}

