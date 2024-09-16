namespace AplicacionWeb.Models
{
    public class EventoCalendarioModel
    {
        public string Dia { get; set; }
        public string Inicio { get; set; }
        public string Fin { get; set; }
        public string Descripcion { get; set; }

        public EventoCalendarioModel() { }

        public EventoCalendarioModel(string dia, string inicio, string fin, string descripcion)
        {
            Dia = dia;
            Inicio = inicio;
            Fin = fin;
            Descripcion = descripcion;
        }
    }
}
