using Face.LogicaDeNegocio;
using Face.UserInterface.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Face.EntidadesDeNegocio;


namespace Face.UserInterface.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EmpleadosBL _empleadosBL = new EmpleadosBL();
        private readonly AsistenciasBL _asistenciasBL = new AsistenciasBL();

        [HttpGet]
        public async Task<IActionResult> GetDashboardMetrics()
        {
            var totalEmpleados = (await _empleadosBL.ObtenerTodosAsync()).Count;
            var totalAsistencias = (await _asistenciasBL.ObtenerTodosAsync()).Count;

            return Json(new { totalEmpleados, totalAsistencias });
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerComentarios()
        {
            // Usamos ObtenerTodosAsync para incluir empleados
            var comentarios = await _asistenciasBL.ObtenerTodosAsync();

            var datos = comentarios.Select(c => new
            {
                Empleado = c.Empleados?.Nombre ?? "Desconocido", // Aseguramos que Empleados no sea null
                Comentario = c.Comentarios ?? "Sin comentario"
            }).ToList();

            return Json(datos);
        }



        [HttpPost]
        public async Task<IActionResult> GuardarComentario([FromBody] ComentarioDto comentarioDto)
        {
            var empleado = await _empleadosBL.ObtenerPorNombreAsync(comentarioDto.Empleado);

            if (empleado == null)
            {
                return Json(new { success = false, message = "Empleado no encontrado" });
            }

            var asistencia = new Asistencias
            {
                EmpleadosId = empleado.Id,
                Comentarios = comentarioDto.Comentario,
                Fecha = DateTime.Now,
                Tipo = "Reconocimiento",
                EstadoReconocimiento = "Exitoso"
            };

            await _asistenciasBL.CrearAsync(asistencia);
            return Json(new { success = true });
        }

        public class ComentarioDto
        {
            public string Empleado { get; set; }
            public string Comentario { get; set; }
        }

        public IActionResult Inicio()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
