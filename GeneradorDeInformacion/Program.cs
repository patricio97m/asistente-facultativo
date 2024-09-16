using GeneradorDeInformacion.Managers;
using GeneradorDeInformacion.Managers.Interfaces;

namespace Program
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("----- Iniciando generación de archivo -----");

            IMgGenerador generador = new MgGeneradorPromedios(100000);

            generador.Generar();
            generador.CrearCsv();

            Console.WriteLine("----- Generación de archivo finalizada -----");
            Console.WriteLine();
        }
    }
}



