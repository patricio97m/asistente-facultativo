using Entidades;
namespace AplicacionWeb.Models
{
    public class DiaModel
    {
        public string? Nombre { get; set; }
        public List<MateriaSeleccionadaData>? MateriasQueCursa {  get; set; } = new List<MateriaSeleccionadaData> { };
        public string InicioHorarioDormir { get; set; }
        public string FinHorarioDormir { get; set; }
        public string InicioHorarioLaboral { get; set; }
        public string FinHorarioLaboral { get; set; }
        public string InicioMayorTiempoLibre { get; set; }
        public string FinMayorTiempoLibre { get; set; }
        public string InicioPrediccionHorarioEstudio { get; set; }
        public string FinPrediccionHorarioEstudio { get; set; }

        public DiaModel()
        {

        }

        public DiaModel(string nombre)
        {
            Nombre = nombre;
        }

    }

    public class SemanaModel
    {
        public DiaModel Lunes { get; set; }
        public DiaModel Martes { get; set; }
        public DiaModel Miercoles { get; set; }
        public DiaModel Jueves { get; set; }
        public DiaModel Viernes { get; set; }
        public DiaModel Sabado { get; set; }
        public DiaModel Domingo { get; set; }
        public List<MateriaSeleccionadaData> MateriasTotales { get; set; } = new List<MateriaSeleccionadaData>();

        public SemanaModel()
        {
            Lunes = new DiaModel("Lunes");
            Martes = new DiaModel("Martes");
            Miercoles = new DiaModel("Miércoles");
            Jueves = new DiaModel("Jueves");
            Viernes = new DiaModel("Viernes");
            Sabado = new DiaModel("Sábado");
            Domingo = new DiaModel("Domingo");
        }
    }
}
