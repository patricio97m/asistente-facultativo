using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Entidades
{
    public class BloqueHorario
    {
        public TimeSpan Inicio {  get; set; }
        public TimeSpan Fin {  get; set; }
        public string Descripcion { get; set; }

        public BloqueHorario() { }

        public BloqueHorario(TimeSpan inicio, TimeSpan fin)
        {
            Inicio = inicio;
            Fin = fin;
        }

        public BloqueHorario(TimeSpan inicio, TimeSpan fin, string descripcion)
        {
            Inicio = inicio;
            Fin = fin;
            Descripcion = descripcion;
        }

        public override string ToString()
        {
            return $"Inicio: {Inicio}, Fin: {Fin}";
        }
    }
}
