using AsistenteFacultativo;
using GeneradorDeInformacion;

namespace Program
{
    public class Program
    {
        public static void Main()
        {
            //Console.WriteLine("IdAlumno | PromedioPromocionAlumno | CantMateriasElegidas | PromedioPromocionOtros | IdMateria | PromedioPromocionMateria | PrediccionPromocion");
            //for (int i = 0; i < 100; i++)
            //{
            //    GenerarYPredecir();
            //    Console.WriteLine();
            //}

            Console.WriteLine("IdAlumno | PromedioPromocionAlumno | CantMateriasElegidas | PromedioPromocionOtros | IdMateria | PromedioPromocionMateria | InicioiHorarioLibre | FinHorarioLibre | PrediccionInicioHorario | PrediccionFinHorario");
            for (int i = 0; i < 100; i++)
            {
                GenerarYPredecirCalendario();
            }

            Console.Write("Para cerrar escriba un caracter: ");
            Console.ReadLine();
        }

        public static void GenerarYPredecir()
        {
            Random random = new Random();

            int idAlumno = random.Next(1, 11000);
            float promedioPromocionAlumno = (float)(random.NextDouble() * 10);
            int cantMateriasElegidas = Convert.ToInt32(random.NextSingle() * 4 + 1);
            float promedioPromocionOtros = 4.3F;
            int idMateria = random.Next(1, 5000);
            float promedioPromocionMateria = (float)(random.NextDouble() * 7 + 3);

            var sampleData = new Modelo.ModelInput()
            {
                IdAlumno = idAlumno,
                PromedioPromocionAlumno = promedioPromocionAlumno,
                CantMateriasElegidas = cantMateriasElegidas,
                PromedioPromocionOtros = promedioPromocionOtros,
                IdMateria = idMateria,
                PromedioPromocionMateria = promedioPromocionMateria,
            };

            var result = Modelo.Predict(sampleData);

            Console.WriteLine($"{idAlumno} | {promedioPromocionAlumno.ToString("F1").Replace(',', '.')} | {cantMateriasElegidas} | {promedioPromocionOtros.ToString("F1").Replace(',', '.')} | {idMateria} | {promedioPromocionMateria.ToString("F1").Replace(',', '.')} | {result.PredictedLabel}");
        }
        public static void GenerarYPredecirCalendario()
        {
            Random random = new Random();

            int idAlumno = random.Next(1, 11000);
            float promedioPromocionAlumno = (float)(random.NextDouble() * 10);
            int cantMateriasElegidas = Convert.ToInt32(random.NextSingle() * 4 + 1);
            float promedioPromocionOtros = 4.3F;
            int idMateria = random.Next(1, 5000);
            float promedioPromocionMateria = (float)(random.NextDouble() * 7 + 3);
            float horarioInicioMayorTiempoLibre = (float)(random.NextDouble() * 12 + 1);
            float horarioFinMayorTiempoLibre = (float)horarioInicioMayorTiempoLibre + random.Next(1, 9);

            //Load sample data
            var sampleData = new ModeloCalendario.ModelInput()
            {
                Id = idAlumno,
                PromedioPromocionAlumno = promedioPromocionAlumno,
                CantMateriasElegidas = cantMateriasElegidas,
                MenorPromedioPromocionMateria = (float)(random.NextDouble() * 7 + 3),
                IdMateria = idMateria,
                PromedioPromocionMateria = promedioPromocionMateria,
                Dia = @"Domingo",
                InicioMayorTiempoLibre = horarioInicioMayorTiempoLibre,
                FinMayorTiempoLibre = horarioFinMayorTiempoLibre,
                InicioPrediccionHorarioEstudio = horarioInicioMayorTiempoLibre,
            };

            //Load model and predict output
            var result = ModeloCalendario.Predict(sampleData);

            Console.WriteLine($"{idAlumno} | {promedioPromocionAlumno.ToString("F1").Replace(',', '.')} | {cantMateriasElegidas} | {promedioPromocionOtros.ToString("F1").Replace(',', '.')} | {idMateria} | {promedioPromocionMateria.ToString("F1").Replace(',', '.')} | {result.InicioMayorTiempoLibre.ToString("F1").Replace(',', '.')} | {result.FinMayorTiempoLibre.ToString("F1").Replace(',', '.')} | {result.InicioPrediccionHorarioEstudio.ToString("F1").Replace(',', '.')} | {result.Score.ToString("F1").Replace(',', '.')}");
        }
    }
}