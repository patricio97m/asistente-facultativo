using AplicacionWeb.Models;
using AsistenteFacultativo;
using Entidades;
using EntityFramework;
using EntityFramework.Clases;
using System.Collections.Generic;
using System.Text.Json;

namespace Services
{
    public interface ICalendarService
    {
        List<EventoCalendarioModel> GenerarListaEventosPorDia(SemanaModel materias);
        SemanaModel getMateriasxDia(List<MateriaSeleccionadaData> datos);
    }
    public class CalendarService : ICalendarService
    {
        private Contexto _ctx;
        public CalendarService(Contexto context)
        {
            _ctx = context;
        }

        public SemanaModel getMateriasxDia(List<MateriaSeleccionadaData> materias)
        {
            SemanaModel semanaModel = new SemanaModel();
            foreach (var materia in materias)
            {
                EntityFramework.Clases.Materia materiaDb = _ctx.Materias.First(m => m.ID == materia.id);
                var comisiones = JsonSerializer.Deserialize<List<Comision>>(materiaDb.Comisiones);
                semanaModel.MateriasTotales = materias;
                foreach (var comision in comisiones)
                {
                    if (comision.Id == materia.comision)
                    {
                        foreach (var diayhorario in comision.diasyhorarios)
                        {
                            var materiaxdia = materia;
                            diayhorario.dia = diayhorario.dia.ToLower();
                            if (diayhorario.dia == "lunes")
                            {
                                var horario = diayhorario.horario.Split('-');
                                materiaxdia.inicio = horario[0];
                                materiaxdia.fin = horario[1];
                                semanaModel.Lunes.MateriasQueCursa.Add(materiaxdia);
                            }
                            if (diayhorario.dia == "martes")
                            {
                                var horario = diayhorario.horario.Split('-');
                                materiaxdia.inicio = horario[0];
                                materiaxdia.fin = horario[1];
                                semanaModel.Martes.MateriasQueCursa.Add(materiaxdia);
                            }
                            if (diayhorario.dia == "miercoles")
                            {
                                var horario = diayhorario.horario.Split('-');
                                materiaxdia.inicio = horario[0];
                                materiaxdia.fin = horario[1];
                                semanaModel.Miercoles.MateriasQueCursa.Add(materiaxdia);
                            }
                            if (diayhorario.dia == "jueves")
                            {
                                var horario = diayhorario.horario.Split('-');
                                materiaxdia.inicio = horario[0];
                                materiaxdia.fin = horario[1];
                                semanaModel.Jueves.MateriasQueCursa.Add(materiaxdia);
                            }
                            if (diayhorario.dia == "viernes")
                            {
                                var horario = diayhorario.horario.Split('-');
                                materiaxdia.inicio = horario[0];
                                materiaxdia.fin = horario[1];
                                semanaModel.Viernes.MateriasQueCursa.Add(materiaxdia);
                            }
                            if (diayhorario.dia == "sabado")
                            {
                                var horario = diayhorario.horario.Split('-');
                                materiaxdia.inicio = horario[0];
                                materiaxdia.fin = horario[1];
                                semanaModel.Sabado.MateriasQueCursa.Add(materiaxdia);
                            }
                            if (diayhorario.dia == "domingo")
                            {
                                var horario = diayhorario.horario.Split('-');
                                materiaxdia.inicio = horario[0];
                                materiaxdia.fin = horario[1];
                                semanaModel.Domingo.MateriasQueCursa.Add(materiaxdia);
                            }
                        }
                    }
                    else continue;
                }
            }
            return semanaModel;
        }

        private float CalcularMateriaConMenorPromedioPromocion(List<MateriaSeleccionadaData> materiasElegidas)
        {
            float valorMinimo = float.MaxValue;
            MateriaSeleccionadaData materiaConValorMinimo = null;

            foreach (var materia in materiasElegidas)
            {
                if (materia.promedioPromocionMateria < valorMinimo)
                {
                    valorMinimo = (float) materia.promedioPromocionMateria;
                    materiaConValorMinimo = materia;
                }
            }

            return (float) materiaConValorMinimo.promedioPromocionMateria;
        }

        private List<BloqueHorario> IdentificarTiempoDisponible(DiaModel dia)
        {
            List<BloqueHorario> listaHorarios = new List<BloqueHorario>();
            List<BloqueHorario> tiempoDisponible = new List<BloqueHorario>();
            BloqueHorario anterior = new BloqueHorario(new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 0));

            listaHorarios.Add(anterior);
            listaHorarios.Add(new BloqueHorario(TimeSpan.Parse(dia.InicioHorarioDormir), TimeSpan.Parse(dia.FinHorarioDormir)));

            if (!TimeSpan.Parse(dia.InicioHorarioLaboral).Equals(anterior.Inicio))
                listaHorarios.Add(new BloqueHorario(TimeSpan.Parse(dia.InicioHorarioLaboral), TimeSpan.Parse(dia.FinHorarioLaboral)));

            if (dia.MateriasQueCursa.Count > 0)
            {
                foreach (var materia in dia.MateriasQueCursa)
                {
                    listaHorarios.Add(new BloqueHorario(TimeSpan.Parse(materia.inicio), TimeSpan.Parse(materia.fin)));
                }
            }
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

        private BloqueHorario CalcularMayorTiempoDisponible(DiaModel dia)
        {
            BloqueHorario mayorTiempoDisponible = new BloqueHorario();
            TimeSpan mayorDuracion = new TimeSpan(0, 0, 0);

            List<BloqueHorario> tiempoDisponible = IdentificarTiempoDisponible(dia);

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

        public List<EventoCalendarioModel> GenerarListaEventosPorDia(SemanaModel semana)
        {
            List<EventoCalendarioModel> listaEventosPorSemana = new List<EventoCalendarioModel>();

            listaEventosPorSemana.AddRange(CrearEventosPorDia(semana.Lunes, semana.MateriasTotales));
            listaEventosPorSemana.AddRange(CrearEventosPorDia(semana.Martes, semana.MateriasTotales));
            listaEventosPorSemana.AddRange(CrearEventosPorDia(semana.Miercoles, semana.MateriasTotales));
            listaEventosPorSemana.AddRange(CrearEventosPorDia(semana.Jueves, semana.MateriasTotales));
            listaEventosPorSemana.AddRange(CrearEventosPorDia(semana.Viernes, semana.MateriasTotales));
            listaEventosPorSemana.AddRange(CrearEventosPorDia(semana.Sabado, semana.MateriasTotales));
            listaEventosPorSemana.AddRange(CrearEventosPorDia(semana.Domingo, semana.MateriasTotales));

            return listaEventosPorSemana;
        }

        private List<EventoCalendarioModel> CrearEventosPorDia(DiaModel dia, List<MateriaSeleccionadaData> listaMateriaTotales)
        {
            List<EventoCalendarioModel> listaEventosPorDia = new List<EventoCalendarioModel>();

            TimeSpan tiempoVacio = TimeSpan.Zero;
            listaEventosPorDia.Add(CrearEvento(dia.Nombre, TimeSpan.Parse(dia.InicioHorarioDormir), TimeSpan.Parse(dia.FinHorarioDormir), "Duermo"));
            if (TimeSpan.Parse(dia.InicioHorarioLaboral) != tiempoVacio && TimeSpan.Parse(dia.FinHorarioLaboral) != tiempoVacio)
                listaEventosPorDia.Add(CrearEvento(dia.Nombre, TimeSpan.Parse(dia.InicioHorarioLaboral), TimeSpan.Parse(dia.FinHorarioLaboral), "Trabajo"));
            if (dia.MateriasQueCursa.Count > 0)
            {
                foreach (var materia in dia.MateriasQueCursa)
                {
                    materia.inicio = materia.inicio + ":00";
                    materia.fin = materia.fin + ":00";
                   listaEventosPorDia.Add(CrearEvento(dia.Nombre, TimeSpan.Parse(materia.inicio), TimeSpan.Parse(materia.fin), $"Curso: {materia.nombre}"));
                }
            }

            BloqueHorario mayorTiempLibre = CalcularMayorTiempoDisponible(dia);

            dia.InicioMayorTiempoLibre = mayorTiempLibre.Inicio.ToString();
            dia.FinMayorTiempoLibre = mayorTiempLibre.Fin.ToString();

            List<BloqueHorario> tiempoEstudio = CalcularTiempoEstudio(dia, listaMateriaTotales);

            if(tiempoEstudio.Count > 0)
            {
                foreach(var bloque in tiempoEstudio)
                {
                    listaEventosPorDia.Add(CrearEvento(dia.Nombre, bloque.Inicio, bloque.Fin, $"Estudio: {bloque.Descripcion}"));
                }
            }

            return listaEventosPorDia;
        }

        private List<BloqueHorario> CalcularTiempoEstudio(DiaModel dia, List<MateriaSeleccionadaData> listaMaterias)
        {
            List<BloqueHorario> listaBloqueHorarioPorMateria = new List<BloqueHorario>();
            var promedioPromocionAlumno = _ctx.ReporteAlumnos
                        .Where(r => r.Promocionable == true && r.Alumno.ID == 2)
                        .Average(r => r.NotaFinal);
            var promedioPromocionOtros = _ctx.ReporteAlumnos
                    .Where(r => r.Promocionable == true && r.Alumno.ID != 2)
                    .Average(r => r.NotaFinal);
            TimeSpan inicioBloque = TimeSpan.Parse(dia.InicioMayorTiempoLibre);
            var materiaMenorPromedioPromocion = CalcularMateriaConMenorPromedioPromocion(listaMaterias);

            listaMaterias = listaMaterias.OrderBy(x=>x.promedioPromocionMateria).ToList();

            TimeSpan finBloque = new TimeSpan(0, 0, 0);

            foreach (var materia in listaMaterias)
            {
                if(TimeSpan.Parse(dia.FinMayorTiempoLibre) > inicioBloque)
                {
                    var sampleData = new AplicacionWeb.ModeloCalendario.ModelInput()
                    {
                        Id = 1F,
                        PromedioPromocionAlumno = (float) promedioPromocionAlumno,
                        CantMateriasElegidas = listaMaterias.Count(),
                        MenorPromedioPromocionMateria = materiaMenorPromedioPromocion,
                        IdMateria = listaMaterias.First().id,
                        PromedioPromocionMateria = (float) materia.promedioPromocionMateria,
                        Dia = dia.Nombre,
                        InicioMayorTiempoLibre = inicioBloque.Hours,
                        FinMayorTiempoLibre = TimeSpan.Parse(dia.FinMayorTiempoLibre).Hours,
                        InicioPrediccionHorarioEstudio = inicioBloque.Hours,
                    };

                    var result = AplicacionWeb.ModeloCalendario.Predict(sampleData);

                    finBloque = ConvertirIntHorasATimeSpan(result.Score);
                    if((finBloque > inicioBloque))
                        listaBloqueHorarioPorMateria.Add(new BloqueHorario(inicioBloque, finBloque, materia.nombre)); 
                }
                inicioBloque = finBloque;
            }
            return listaBloqueHorarioPorMateria;
        }

        private TimeSpan ConvertirIntHorasATimeSpan(float score)
        {
            double totalHoras = score;
            int horas = (int)totalHoras;
            int minutos = (int)((totalHoras - horas) * 60);
            TimeSpan finBloque = new TimeSpan(horas, minutos, 0);

            return finBloque;
        }

        private EventoCalendarioModel CrearEvento(string nombre, TimeSpan inicioHorarioLaboral, TimeSpan finHorarioLaboral, string descripcion)
        {
            return new EventoCalendarioModel(nombre, inicioHorarioLaboral.ToString(), finHorarioLaboral.ToString(), descripcion);
        }
    }

}
