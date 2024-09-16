using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ReporteItinerarioDelDia
    {
        public BigInteger Id { get; set; }
        public string Dia { get; set; }
        public byte HoyCursa { get; set; }
        public byte HoyTrabaja { get; set; }
        public int CantidadDeMateriasElegidas { get; set; }
        public float MenorPromedioPromocionMateria { get; set; }
        public Materia Materia { get; set; }
        public float PromedioPromocionAlumno { get; set; }
        public TimeSpan InicioHorarioDormir { get; set; }
        public TimeSpan FinHorarioDormir { get; set; }
        public TimeSpan InicioHorarioMateria { get; set; }
        public TimeSpan FinHorarioMateria { get; set; }
        public TimeSpan InicioHorarioLaboral { get; set; }
        public TimeSpan FinHorarioLaboral { get; set; }
        public TimeSpan InicioMayorTiempoLibre { get; set; }
        public TimeSpan FinMayorTiempoLibre { get; set; }
        public TimeSpan InicioPrediccionHorarioEstudio { get; set; }
        public TimeSpan FinPrediccionHorarioEstudio { get; set; }
        public ReporteItinerarioDelDia Clone()
        {
            return new ReporteItinerarioDelDia()
            {
                Id = Id,
                Dia = Dia,
                HoyCursa = HoyCursa,
                HoyTrabaja = HoyTrabaja,
                CantidadDeMateriasElegidas = CantidadDeMateriasElegidas,
                MenorPromedioPromocionMateria = MenorPromedioPromocionMateria,
                Materia = Materia,
                PromedioPromocionAlumno = PromedioPromocionAlumno,
                InicioHorarioDormir = InicioHorarioDormir,
                FinHorarioDormir = FinHorarioDormir,
                InicioHorarioMateria = InicioHorarioMateria,
                FinHorarioMateria = FinHorarioMateria,
                InicioHorarioLaboral = InicioHorarioLaboral,
                FinHorarioLaboral = FinHorarioLaboral,
                InicioMayorTiempoLibre = InicioMayorTiempoLibre,
                FinMayorTiempoLibre = FinMayorTiempoLibre,
                InicioPrediccionHorarioEstudio = InicioPrediccionHorarioEstudio,
                FinPrediccionHorarioEstudio = FinPrediccionHorarioEstudio
            };
        }
    }
}
