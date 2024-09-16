using AplicacionWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json;

namespace AplicacionWeb.Controllers
{
    public class CalendarController : Controller
    {
        public ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpPost]
        public IActionResult Index(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    var materias = JsonSerializer.Deserialize<SemanaModel>(jsonData);
                    ViewBag.ListaEventos = _calendarService.GenerarListaEventosPorDia(materias).OrderBy(e => e.Inicio).ToList();
                }
                catch (JsonException ex)
                {
                    // Manejar el error de deserialización
                    ModelState.AddModelError("", "Error al deserializar los datos JSON.");
                }
                catch (Exception ex)
                {
                    // Manejar otros posibles errores
                    ModelState.AddModelError("", "Ocurrió un error inesperado.");
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult calendarForm(string materias)
        {
            List<MateriaSeleccionadaData> materiasSeleccionadas = JsonSerializer.Deserialize<List<MateriaSeleccionadaData>>(materias);
            SemanaModel semana = _calendarService.getMateriasxDia(materiasSeleccionadas);
            return View(semana);
        }

    }
}
