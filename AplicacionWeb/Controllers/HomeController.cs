using AplicacionWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AplicacionWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIntraconsultaService _intraconsultaService;

        public HomeController(ILogger<HomeController> logger, IIntraconsultaService intraconsultaService)
        {
            _logger = logger;
            _intraconsultaService = intraconsultaService;
        }

        public IActionResult Index()
        {
            var materias = _intraconsultaService.GetMaterias();
            var materiasSeleccionadas = new List<MateriaSeleccionada>();
            ViewBag.Data = materiasSeleccionadas;
            return View(materias);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
