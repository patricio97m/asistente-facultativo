using AplicacionWeb.Models;
using AsistenteFacultativo;
using Entidades;
using EntityFramework;
using EntityFramework.Clases;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Services
{
    public interface IIntraconsultaService
    {
        List<MateriaModel> GetMaterias();
        List<MateriaSeleccionadaData> getProcessData(List<MateriaSeleccionada> datos);
    }
    public class IntraconsultaService : IIntraconsultaService
    {
        private Contexto _ctx;
        public IntraconsultaService(Contexto context)
        {
            _ctx = context;
        }
        public List<MateriaModel> GetMaterias()
        {
            var materias = _ctx.Materias.ToList();
            var materiasProcesadas = new List<MateriaModel>();

            foreach (var materia in materias)
            {
                var comisionesJson = materia.Comisiones;
                var materiaModel = new MateriaModel();

                materiaModel.ID = materia.ID;
                materiaModel.Nombre = materia.Nombre;
                materiaModel.CargaHoraria = materia.CargaHoraria;

                if (comisionesJson == null) continue;

                var comisiones = JsonSerializer.Deserialize<List<Comision>>(comisionesJson);

                if (comisiones == null) continue;

                foreach (var comision in comisiones)
                {
                    var dias = "";

                    if(comision.diasyhorarios == null) continue;

                    foreach (var diayhorario in comision.diasyhorarios)
                    {
                        dias += diayhorario.dia + ", " + diayhorario.horario + " ";
                    }

                    materiaModel?.Comisiones?.Add(new _Comision(comision.Id, dias));
                }
                if(materiaModel == null)continue;
                materiasProcesadas.Add(materiaModel);
            }

            return materiasProcesadas;
        }

        public List<MateriaSeleccionadaData> getProcessData(List<MateriaSeleccionada> materias)
        {
            var alumnoReportes = _ctx.ReporteAlumnos
                                     .Where(r => r.AlumnoID == 2);

            int totalRegistros = alumnoReportes.Count();
            int totalPromocionable = alumnoReportes.Count(r => r.Promocionable);
            double promedioPromocionAlumno = (double)totalPromocionable / totalRegistros * 10;

            var otrosAlumnosReportes = _ctx.ReporteAlumnos
                    .Where(r => r.AlumnoID != 2);

            int totalRegistrosOtrosAlumno = otrosAlumnosReportes.Count();
            int totalPromocionableOtrosAlumno = otrosAlumnosReportes.Count(r => r.Promocionable);
            double promedioPromocionOtros = (double)totalPromocionableOtrosAlumno / totalRegistrosOtrosAlumno * 10;

            var materiasSeleccionadas = new List<MateriaSeleccionadaData>();

            if (materias.Count > 0)
            {

                foreach (var materia in materias)
                {
                    var id = Int32.Parse(materia.Id);
                    var Comision = Int32.Parse(materia.Comision);
                    var PromedioPromocionMateria = _ctx.ReporteAlumnos
                        .Where(r => r.Materia.ID == id)
                        .Average(r => r.NotaFinal);
                    PromedioPromocionMateria = Math.Round(PromedioPromocionMateria, 2);

                    MateriaSeleccionadaData materiaSeleccionada = new MateriaSeleccionadaData(id, Comision, PromedioPromocionMateria);
                    materiaSeleccionada.prediccionPromocion = PredecirPromocionMateriaPorAlumno(materiaSeleccionada, materias.Count, (float)promedioPromocionAlumno, (float)promedioPromocionOtros);
                    materiaSeleccionada.nombre = _ctx.Materias
                        .First(x => x.ID == id)
                        .Nombre;

                    materiasSeleccionadas.Add(materiaSeleccionada);
                }
            }

            return materiasSeleccionadas;
        }

        public bool PredecirPromocionMateriaPorAlumno(MateriaSeleccionadaData materia, int cantMateriasElegidas, float promedioPromocionAlumno, float promedioPromocionOtros)
        {
            //Load sample data
            var sampleData = new AplicacionWeb.Modelo.ModelInput()
            {
                IdAlumno = 1F,
                PromedioPromocionAlumno = promedioPromocionAlumno,
                CantMateriasElegidas = cantMateriasElegidas,
                PromedioPromocionOtros = promedioPromocionOtros,
                IdMateria = materia.id,
                PromedioPromocionMateria = (float)materia.promedioPromocionMateria,
            };

            //Load model and predict output
            var result = AplicacionWeb.Modelo.Predict(sampleData);

            return (result.PredictedLabel == 1) ? true : false;
        }

    }

}
