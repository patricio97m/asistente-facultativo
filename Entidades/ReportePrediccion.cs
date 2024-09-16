using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ReportePrediccion
    {
        public BigInteger Id { get; set; }
        public BigInteger IdAlumno { get; set; }
        public int CantMateriasElegidas { get; set; }
        public int IdMateria { get; set; }
        public float PromedioPromocionAlumno { get; set; }
        public float PromedioPromocionOtros { get; set; }
        public float PromedioPromocionMateria { get; set; }
        public int PrediccionPromocion { get; set; }
    }
}
