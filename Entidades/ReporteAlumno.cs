using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ReporteAlumno
    {
        public BigInteger Id { get; set; }
        public Materia Materia {  get; set; }
        public Alumno Alumno { get; set; }
        public int NotaFinal {  get; set; }
        public bool Promocionable { get; set; }
    }
}
