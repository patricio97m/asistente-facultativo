using AplicacionWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json;

namespace AplicacionWeb.Controllers
{
    public class IntraconsultaController : Controller
    {
        IIntraconsultaService _intraconsultaService;

        public IntraconsultaController(IIntraconsultaService intraconsultaService)
        {
            _intraconsultaService = intraconsultaService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PrediccionMaterias(string MateriasSeleccionadas)
        {
            if (MateriasSeleccionadas != null) {
                var materiasSeleccionadas = JsonSerializer.Deserialize<List<MateriaSeleccionada>>(MateriasSeleccionadas); 
                var data = _intraconsultaService.getProcessData(materiasSeleccionadas);
                ViewBag.Data = data;
                ViewBag.DataSerializada = JsonSerializer.Serialize(data);
            }

            return View();
        }

    }
}
