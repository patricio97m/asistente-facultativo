using Entidades;
using GeneradorDeInformacion.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GeneradorDeInformacion.Managers
{
    internal class MgGeneradorCalendario : IMgGenerador
    {
        private int cantRegistros;
        List<ReporteItinerarioDelDia> reportesPorAlumno = new List<ReporteItinerarioDelDia>();

        public MgGeneradorCalendario(int cantidad)
        {
            cantRegistros = cantidad;
        }

        public void Generar()
        {
            Random random = new Random();
            Dictionary<int, string> dias = GenerarDictionaryDia();

            for (int i = 0; i < cantRegistros; i++)
            {
                BigInteger idReporte = GenerarIdReporte(random);
                List<Materia> materiasElegidas = GenerarCantidadMateriasElegidas(random);
                int cantidadDeMateriasElegidas = materiasElegidas.Count;
                float menorPromedioPromocionMateria = CalcularMateriaConMenorPromedioPromocion(materiasElegidas);
                float promedioPromocionAlumno = GenerarPromocionAlumno(random);
                float promedioPromocionMateria = GenerarPromocionMateria(random);

                BloqueHorario horarioMateria = GenerarHorarioMateria(random);

                BloqueHorario horarioLaboral = GenerarHorarioLaboral(random, horarioMateria);

                TimeSpan tiempoVacio = new TimeSpan(0, 0, 0);

                ReporteItinerarioDelDia itinerario = new();

                itinerario.Id = idReporte;
                itinerario.Dia = dias[random.Next(1,8)];
                itinerario.HoyCursa = (byte) random.Next(0,2);
                itinerario.HoyTrabaja = (byte) random.Next(0,2);
                itinerario.CantidadDeMateriasElegidas = cantidadDeMateriasElegidas;
                itinerario.MenorPromedioPromocionMateria = menorPromedioPromocionMateria;
                itinerario.Materia = materiasElegidas.First(m => m.PromedioPromocion == menorPromedioPromocionMateria);
                itinerario.PromedioPromocionAlumno = promedioPromocionAlumno;

                itinerario.InicioHorarioMateria = (itinerario.HoyCursa == 1) ? horarioMateria.Inicio : tiempoVacio;
                itinerario.FinHorarioMateria = (itinerario.HoyCursa == 1) ? horarioMateria.Fin : tiempoVacio;

                itinerario.InicioHorarioLaboral = (itinerario.HoyTrabaja == 1) ? horarioLaboral.Inicio : tiempoVacio;
                itinerario.FinHorarioLaboral = (itinerario.HoyTrabaja == 1) ? horarioLaboral.Fin : tiempoVacio;

                BloqueHorario horarioDormir = GenerarHorarioDormir(random, itinerario, tiempoVacio);

                itinerario.InicioHorarioDormir = horarioDormir.Inicio;
                itinerario.FinHorarioDormir = horarioDormir.Fin;

                BloqueHorario horarioMayorTiempoDisponibleEnElDia = CalcularMayorTiempoDisponible(itinerario);

                itinerario.InicioMayorTiempoLibre = horarioMayorTiempoDisponibleEnElDia.Inicio;
                itinerario.FinMayorTiempoLibre = horarioMayorTiempoDisponibleEnElDia.Fin;

                List<BloqueHorario> listaHorariosEstudio = new List<BloqueHorario>();

                TimeSpan inicioMayorTiempoLibreOriginal = itinerario.InicioMayorTiempoLibre;

                do
                {
                    BloqueHorario horarioEstudio = GenerarHorarioEstudio(itinerario);

                    itinerario.InicioPrediccionHorarioEstudio = horarioEstudio.Inicio;
                    itinerario.FinPrediccionHorarioEstudio = horarioEstudio.Fin;

                    reportesPorAlumno.Add(itinerario.Clone());

                    if(materiasElegidas.Count > 1)
                    {
                        materiasElegidas.Remove(itinerario.Materia);
                        itinerario.Materia = materiasElegidas[random.Next(materiasElegidas.Count)];
                    }
                    else
                    {
                        break;
                    }
                    itinerario.InicioMayorTiempoLibre = horarioEstudio.Fin;
                }
                while (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre > itinerario.FinPrediccionHorarioEstudio - itinerario.InicioPrediccionHorarioEstudio);
            }
        }

        private BloqueHorario CalcularMayorTiempoDisponible(ReporteItinerarioDelDia itinerario)
        {
            BloqueHorario mayorTiempoDisponible = new BloqueHorario();
            TimeSpan mayorDuracion = new TimeSpan(0, 0, 0);

            List<BloqueHorario> tiempoDisponible = IdentificarTiempoDisponible(itinerario);

            foreach (BloqueHorario tiempo in tiempoDisponible)
            {
                TimeSpan duracion = tiempo.Fin - tiempo.Inicio;

                if (duracion > mayorDuracion)
                {
                    mayorDuracion = duracion;
                    mayorTiempoDisponible = tiempo;
                }
            }

            return mayorTiempoDisponible;
        }

        private List<BloqueHorario> IdentificarTiempoDisponible(ReporteItinerarioDelDia itinerario)
        {
            List<BloqueHorario> listaHorarios = new List<BloqueHorario>();
            List<BloqueHorario> tiempoDisponible = new List<BloqueHorario>();
            BloqueHorario anterior = new BloqueHorario(new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 0));

            listaHorarios.Add(anterior);
            listaHorarios.Add(new BloqueHorario(itinerario.InicioHorarioDormir, itinerario.FinHorarioDormir));
            if (!itinerario.InicioHorarioLaboral.Equals(anterior.Inicio))
                listaHorarios.Add(new BloqueHorario(itinerario.InicioHorarioLaboral, itinerario.FinHorarioLaboral));
            if (!itinerario.InicioHorarioMateria.Equals(anterior.Inicio))
                listaHorarios.Add(new BloqueHorario(itinerario.InicioHorarioMateria, itinerario.FinHorarioMateria));
            listaHorarios.Add(new BloqueHorario(new TimeSpan(23, 0, 0), new TimeSpan(23, 0, 0)));

            listaHorarios.Sort((h1, h2) => h1.Inicio.CompareTo(h2.Inicio));

            foreach (BloqueHorario horario in listaHorarios)
            {
                if (anterior.Fin < horario.Inicio)
                {
                    tiempoDisponible.Add(new BloqueHorario(anterior.Fin, horario.Inicio));
                }
                anterior = horario;
            }

            return tiempoDisponible;
        }

        private BloqueHorario GenerarHorarioDormir(Random random, ReporteItinerarioDelDia itinerario, TimeSpan tiempoVacio)
        {
            if(itinerario.InicioHorarioLaboral.Equals(tiempoVacio) && itinerario.InicioHorarioMateria.Equals(tiempoVacio))
            {
                return new BloqueHorario(new TimeSpan(0, 0, 0), new TimeSpan(8, 0, 0)); 
            }

            List<TimeSpan> listaTiempo = new List<TimeSpan>();

            listaTiempo.Add(tiempoVacio);
            if(!itinerario.InicioHorarioLaboral.Equals(tiempoVacio))
                listaTiempo.Add(itinerario.InicioHorarioLaboral);
            if(!itinerario.InicioHorarioMateria.Equals(tiempoVacio))
                listaTiempo.Add(itinerario.InicioHorarioMateria);
            listaTiempo.Add(new TimeSpan(23, 0, 0));

            listaTiempo = listaTiempo.OrderBy(x => x).ToList();

            TimeSpan inicioHoraDormir = new TimeSpan(0, 0, 0);

            if (listaTiempo.Count > 2)
                inicioHoraDormir = listaTiempo[1] - new TimeSpan(9, 0, 0);

            if(inicioHoraDormir < tiempoVacio)
            {
                inicioHoraDormir = tiempoVacio;
            }

            TimeSpan finHoraDormir = new TimeSpan(8, 0, 0);

            if (listaTiempo.Count > 2)
                finHoraDormir = listaTiempo[1] - new TimeSpan(2, 0, 0);

            return new BloqueHorario(inicioHoraDormir, finHoraDormir);
        }

        private float CalcularMateriaConMenorPromedioPromocion(List<Materia> materiasElegidas)
        {
            float valorMinimo = float.MaxValue;
            Materia materiaConValorMinimo = null;

            foreach (Materia materia in materiasElegidas)
            {
                if (materia.PromedioPromocion < valorMinimo)
                {
                    valorMinimo = materia.PromedioPromocion;
                    materiaConValorMinimo = materia;
                }
            }

            return materiaConValorMinimo.PromedioPromocion;
        }

        private BloqueHorario GenerarHorarioLaboral(Random random, BloqueHorario horarioMateria)
        {
            BloqueHorario horarioLaboral = new BloqueHorario();
            int horasDeDuracion = random.Next(4, 9);
            List<TimeSpan> dia = new List<TimeSpan>();

            dia.Add(new TimeSpan(0, 0, 0));
            dia.Add(horarioMateria.Inicio);
            dia.Add(horarioMateria.Fin);
            dia.Add(new TimeSpan(23, 59, 59));

            if (dia[1] - dia[0] > dia[3] - dia[2])
            {
                horarioLaboral.Inicio = dia[1] - dia[0] - new TimeSpan(horasDeDuracion + 1, 0, 0);
                horarioLaboral.Fin = horarioLaboral.Inicio + new TimeSpan(horasDeDuracion, 0, 0);
            }
            else
            {
                horarioLaboral.Inicio = dia[3] + new TimeSpan(1, 0, 0);
                horarioLaboral.Fin = horarioLaboral.Inicio + new TimeSpan(horasDeDuracion, 0, 0);
            }
            
            return horarioLaboral;
        }

        private BloqueHorario GenerarHorarioMateria(Random random)
        {
            Dictionary<int, BloqueHorario> horariosMaterias = new Dictionary<int, BloqueHorario>();

            horariosMaterias.Add(1, new BloqueHorario(new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0)));
            horariosMaterias.Add(2, new BloqueHorario(new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0)));
            horariosMaterias.Add(3, new BloqueHorario(new TimeSpan(19, 0, 0), new TimeSpan(22, 0, 0)));

            return horariosMaterias[Convert.ToInt32(random.NextSingle() * 2 + 1)]; 
        }

        private float GenerarPromocionMateria(Random random)
        {
            return random.NextSingle() * 9 + 1;
        }

        private float GenerarPromocionAlumno(Random random)
        {
            return random.NextSingle() * 9 + 1;
        }

        private List<Materia> GenerarCantidadMateriasElegidas(Random random)
        {
            Dictionary<int, Materia> materias = GenerarDictionaryMaterias();
            List<Materia> listaMaterias = new List<Materia>();

            int cantMateriasElegidas = random.Next(1, 6);

            for (int i = 0; i < cantMateriasElegidas; i++)
            {
                int randomMateriaIndex = random.Next(materias.Count);

                Materia materiaElegida = materias.ElementAt(randomMateriaIndex).Value;
                materiaElegida.Id = materias.ElementAt(randomMateriaIndex).Key;

                listaMaterias.Add(materiaElegida);
            }

            return listaMaterias;
        }

        private BigInteger GenerarIdReporte(Random random)
        {
            return random.Next();
        }

        private BloqueHorario DedicarTiempoMateria(BloqueHorario horarioEstudio, ReporteItinerarioDelDia itinerario, TimeSpan tiempoDedicado)
        {
            return new BloqueHorario(itinerario.InicioMayorTiempoLibre, itinerario.InicioMayorTiempoLibre + tiempoDedicado);
        }

        private BloqueHorario GenerarHorarioEstudio(ReporteItinerarioDelDia itinerario)
        {
            BloqueHorario horarioEstudio = new BloqueHorario();
            Materia materiasElegida = itinerario.Materia;
            TimeSpan unaHora = new TimeSpan(1, 0, 0);
            TimeSpan treintaMinutos = new TimeSpan(0, 30, 0);
            BloqueHorario sinTiempo = new BloqueHorario(new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 0));

            switch (itinerario.CantidadDeMateriasElegidas)
            {
                case 5:
                    horarioEstudio = DistribuirTiempoConCincoMaterias(itinerario, horarioEstudio, unaHora, treintaMinutos, sinTiempo);
                    break;
                case 4:
                    horarioEstudio = DistribuirTiempoConCuatroMaterias(itinerario, horarioEstudio, unaHora, treintaMinutos, sinTiempo);
                    break;
                case 3:
                    horarioEstudio = DistribuirTiempoConTresMaterias(itinerario, horarioEstudio, unaHora, treintaMinutos, sinTiempo);
                    break;
                case 2:
                    horarioEstudio = DistribuirTiempoConDosMaterias(itinerario, horarioEstudio, unaHora, treintaMinutos, sinTiempo);
                    break;
                case 1:
                    horarioEstudio = DistribuirTiempoConUnaMateria(itinerario, horarioEstudio);
                    break;
                default:
                    break;
            }
            return horarioEstudio;
        }

        private BloqueHorario DistribuirTiempoConUnaMateria(ReporteItinerarioDelDia itinerario, BloqueHorario horarioEstudio)
        {
            TimeSpan tiempoDedicado = (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre);
            horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, tiempoDedicado);

            return horarioEstudio;
        }

        private BloqueHorario DistribuirTiempoConDosMaterias(ReporteItinerarioDelDia itinerario, BloqueHorario horarioEstudio, TimeSpan unaHora, TimeSpan treintaMinutos, BloqueHorario sinTiempo)
        {
            if (itinerario.PromedioPromocionAlumno <= 5)
            {
                if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 5);
                else if((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 6)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 3);
                else if((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 4)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2);
                else if((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 2)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora);
                else if((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) == unaHora * 1)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) == treintaMinutos)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else
                    horarioEstudio = sinTiempo;
            }
            else
            {
                if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 4);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 6)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 4)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora + treintaMinutos);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 2)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) == unaHora * 1)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) == treintaMinutos)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else
                    horarioEstudio = sinTiempo;
            }

            return horarioEstudio;
        }

        private BloqueHorario DistribuirTiempoConTresMaterias(ReporteItinerarioDelDia itinerario, BloqueHorario horarioEstudio, TimeSpan unaHora, TimeSpan treintaMinutos, BloqueHorario sinTiempo)
        {
            if (itinerario.PromedioPromocionAlumno <= 5)
            {
                if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 3);
                else if(itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 6)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2);
                else if(itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 2)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora);
                else if(itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) == unaHora)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else if((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2 + treintaMinutos);
                else if((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 6)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 1);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= treintaMinutos)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else
                    horarioEstudio = sinTiempo;
            }
            else
            {
                if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2 + treintaMinutos);
                else if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 4)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora + treintaMinutos);
                else if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 2)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora);
                else if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) == unaHora)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2 + treintaMinutos);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 4)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 1);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= treintaMinutos)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else
                    horarioEstudio = sinTiempo;
            }

            return horarioEstudio;
        }

        private BloqueHorario DistribuirTiempoConCuatroMaterias(ReporteItinerarioDelDia itinerario, BloqueHorario horarioEstudio, TimeSpan unaHora, TimeSpan treintaMinutos, BloqueHorario sinTiempo)
        {
            if (itinerario.PromedioPromocionAlumno <= 5)
            {
                if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2 + treintaMinutos);
                else if(itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 6)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2);
                else if(itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 2)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora);
                else if(itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) == unaHora)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else if((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2);
                else if((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 6)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 1);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= treintaMinutos)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else
                    horarioEstudio = sinTiempo;
            }
            else
            {
                if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2);
                else if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 6)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora + treintaMinutos);
                else if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 2)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora);
                else if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) == unaHora)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora + treintaMinutos);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 6)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 1);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= treintaMinutos)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else
                    horarioEstudio = sinTiempo;
            }

            return horarioEstudio;
        }

        private BloqueHorario DistribuirTiempoConCincoMaterias(ReporteItinerarioDelDia itinerario, BloqueHorario horarioEstudio, TimeSpan unaHora, TimeSpan treintaMinutos, BloqueHorario sinTiempo)
        {
            if (itinerario.PromedioPromocionAlumno <= 5)
            {
                if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2 + treintaMinutos);
                else if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 6)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2);
                else if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 2)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora);
                else if(itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) == unaHora)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else if((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora + treintaMinutos);
                else if((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 6)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 1);
                else if ((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= treintaMinutos)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else
                    horarioEstudio = sinTiempo;
            }
            else
            {
                if (itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora * 2 );
                else if(itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 6)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora);
                else if(itinerario.Materia.PromedioPromocion == itinerario.MenorPromedioPromocionMateria && (itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else if((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= unaHora * 10)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, unaHora + treintaMinutos);
                else if((itinerario.FinMayorTiempoLibre - itinerario.InicioMayorTiempoLibre) >= treintaMinutos)
                    horarioEstudio = DedicarTiempoMateria(horarioEstudio, itinerario, treintaMinutos);
                else
                    horarioEstudio = sinTiempo;
            }

            return horarioEstudio;
        }

        private Dictionary<int, Materia> GenerarDictionaryMaterias()
        {
            return new Dictionary<int, Materia>()
            {
                { 2619, new Materia("PROGRAMACION BASICA I") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2620, new Materia("INFORMATICA GENERAL") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2621, new Materia("MATEMATICA GENERAL") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2622, new Materia("INGLES TECNICO I") { PromedioPromocion = GenerarPromocionMateria(new Random())} },
                { 2623, new Materia("PROGRAMACION BASICA II") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2624, new Materia("PROGRAMACION WEB I") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2625, new Materia("BASES DE DATOS I") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2626, new Materia("INTRODUCCION AL DISEÑO GRAFICO EN LA WEB") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2627, new Materia("INGLES TECNICO II") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2628, new Materia("PROGRAMACION WEB II") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2629, new Materia("DISEÑO DE APLICACIONES WEB") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2630, new Materia("VISUALIZACION E INTERFACES") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2631, new Materia("TALLER WEB I") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2632, new Materia("BASE DE DATOS II") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2633, new Materia("PROGRAMACION WEB III") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2634, new Materia("TECNOLOGIA DE REDES") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2635, new Materia("TALLER WEB II") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2636, new Materia("SEGURIDAD Y CALIDAD EN APLICACION WEB") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2637, new Materia("INTRODUCCION A LA ADMINISTRACION DE PROYECTOS") { PromedioPromocion = GenerarPromocionMateria(new Random()) } },
                { 2638, new Materia("TALLER PRACTICO INTEGRADOR") { PromedioPromocion = GenerarPromocionMateria(new Random()) } }
            };
        }

        private Dictionary<int, string> GenerarDictionaryDia()
        {
            return new Dictionary<int, string>()
            {
                { 1, "Lunes" },
                { 2, "Martes" },
                { 3, "Miércoles" },
                { 4, "Jueves" },
                { 5, "Viernes" },
                { 6, "Sábado" },
                { 7, "Domingo" }
            };
        }

        public void CrearCsv()
        {
            string directorioProyecto = Directory.GetCurrentDirectory();
            string directorioInformacion = Path.Combine(directorioProyecto, "data");
            Directory.CreateDirectory(directorioInformacion);

            string carpetaArchivo = Path.Combine(directorioInformacion, "reportePrediccionCalendario.csv");
            using (StreamWriter writer = new StreamWriter(carpetaArchivo))
            {
                writer.WriteLine("Id,PromedioPromocionAlumno,CantMateriasElegidas,MenorPromedioPromocionMateria,IdMateria,PromedioPromocionMateria,Dia,InicioMayorTiempoLibre,FinMayorTiempoLibre,InicioPrediccionHorarioEstudio,FinPrediccionHorarioEstudio");

                foreach (var reporte in reportesPorAlumno.OrderBy(x => x.Id))
                {
                    string promedioPromocionAlumnoSinComa = reporte.PromedioPromocionAlumno.ToString("F1").Replace(',', '.');
                    string promedioPromocionMateriaSinComa = reporte.Materia.PromedioPromocion.ToString("F1").Replace(',', '.');
                    string promedioMenorPromocionMateriaSinComa = reporte.MenorPromedioPromocionMateria.ToString("F1").Replace(',', '.');
                    string InicioMayorTiempoLibre = reporte.InicioMayorTiempoLibre.TotalHours.ToString("F1").Replace(',', '.');
                    string FinMayorTiempoLibre = reporte.FinMayorTiempoLibre.TotalHours.ToString("F1").Replace(',', '.');
                    string InicioPrediccionHorarioEstudio = reporte.InicioPrediccionHorarioEstudio.TotalHours.ToString("F1").Replace(',', '.');
                    string FinPrediccionHorarioEstudio = reporte.FinPrediccionHorarioEstudio.TotalHours.ToString("F1").Replace(',', '.');

                    writer.WriteLine($"{reporte.Id}, {promedioPromocionAlumnoSinComa}, {reporte.CantidadDeMateriasElegidas}, {promedioMenorPromocionMateriaSinComa}, " +
                        $"{reporte.Materia.Id}, {promedioPromocionMateriaSinComa}, {reporte.Dia}, {InicioMayorTiempoLibre}, {FinMayorTiempoLibre}, " +
                        $"{InicioPrediccionHorarioEstudio}, {FinPrediccionHorarioEstudio}");
                }
            }

            Process.Start("explorer.exe", directorioInformacion);
        }
    }
}
