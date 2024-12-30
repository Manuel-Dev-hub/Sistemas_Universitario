using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversidadWEB.Models;

namespace UniversidadWEB.Controllers
{
    public class ConsultasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsultasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Consultas
        public async Task<IActionResult> Index()
        {
            try
            {
                var alumnos = await _context.Alumnos
                    .Where(a => a.Estado)
                    .OrderBy(a => a.apellido)
                    .ThenBy(a => a.Nombres)
                    .ToListAsync();

                ViewBag.Alumnos = new SelectList(
                    alumnos.Select(a => new
                    {
                        AlumnoID = a.AlumnoID,
                        NombreCompleto = $"{a.Nombres} {a.apellido} - {a.Carnet}"
                    }),
                    "AlumnoID",
                    "NombreCompleto"
                );

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar los alumnos: {ex.Message}";
                return View();
            }
        }

        // POST: Consultas/CursosAprobados
        [HttpPost]
        public async Task<IActionResult> CursosAprobados(int? alumnoId)
        {
            try
            {
                if (!alumnoId.HasValue || alumnoId == 0)
                {
                    TempData["ErrorMessage"] = "Debe seleccionar un alumno";
                    return RedirectToAction(nameof(Index));
                }

                var alumno = await _context.Alumnos
                    .FirstOrDefaultAsync(a => a.AlumnoID == alumnoId && a.Estado);

                if (alumno == null)
                {
                    TempData["ErrorMessage"] = "Alumno no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var cursosAprobados = await _context.Notas
                    .Include(n => n.Asignacion)
                        .ThenInclude(a => a.Curso)
                    .Include(n => n.Asignacion)
                        .ThenInclude(a => a.Alumno)
                    .Where(n => n.Asignacion.AlumnoID == alumnoId
                           && n.Estado
                           && n.Calificacion >= 61)
                    .OrderByDescending(n => n.Asignacion.Anio)
                    .ThenBy(n => n.Asignacion.Ciclo)
                    .ThenBy(n => n.Asignacion.Curso.Nombre)
                    .ToListAsync();

                ViewBag.Alumno = alumno;
                ViewBag.TotalCursos = cursosAprobados.Count;
                ViewBag.PromedioGeneral = cursosAprobados.Any()
                    ? Math.Round(cursosAprobados.Average(n => n.Calificacion), 2)
                    : 0;

                // Recargar el SelectList de alumnos
                var alumnos = await _context.Alumnos
                    .Where(a => a.Estado)
                    .OrderBy(a => a.apellido)
                    .ThenBy(a => a.Nombres)
                    .ToListAsync();

                ViewBag.Alumnos = new SelectList(
                    alumnos.Select(a => new
                    {
                        AlumnoID = a.AlumnoID,
                        NombreCompleto = $"{a.Nombres} {a.apellido} - {a.Carnet}"
                    }),
                    "AlumnoID",
                    "NombreCompleto",
                    alumnoId
                );

                return View(cursosAprobados);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar los cursos aprobados: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Consultas/RegistroNotas
        public async Task<IActionResult> RegistroNotas(int? alumnoId)
        {
            try
            {
                if (!alumnoId.HasValue || alumnoId == 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                var alumno = await _context.Alumnos
                    .FirstOrDefaultAsync(a => a.AlumnoID == alumnoId && a.Estado);

                if (alumno == null)
                {
                    TempData["ErrorMessage"] = "Alumno no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var notas = await _context.Notas
                    .Include(n => n.Asignacion)
                        .ThenInclude(a => a.Curso)
                    .Include(n => n.Asignacion)
                        .ThenInclude(a => a.Alumno)
                    .Where(n => n.Asignacion.AlumnoID == alumnoId && n.Estado)
                    .OrderByDescending(n => n.FechaRegistro)
                    .ToListAsync();

                ViewBag.Alumno = alumno;
                ViewBag.TotalNotas = notas.Count;
                ViewBag.PromedioGeneral = notas.Any()
                    ? Math.Round(notas.Average(n => n.Calificacion), 2)
                    : 0;

                // Recargar el SelectList de alumnos para mantener la selección
                var alumnos = await _context.Alumnos
                    .Where(a => a.Estado)
                    .OrderBy(a => a.apellido)
                    .ThenBy(a => a.Nombres)
                    .ToListAsync();

                ViewBag.Alumnos = new SelectList(
                    alumnos.Select(a => new
                    {
                        AlumnoID = a.AlumnoID,
                        NombreCompleto = $"{a.Nombres} {a.apellido} - {a.Carnet}"
                    }),
                    "AlumnoID",
                    "NombreCompleto",
                    alumnoId
                );

                return View(notas);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el registro de notas: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Consultas/ConstanciaNotas
        public async Task<IActionResult> ConstanciaNotas(int? alumnoId)
        {
            try
            {
                if (!alumnoId.HasValue)
                {
                    var alumnos = await _context.Alumnos
                        .Where(a => a.Estado)
                        .OrderBy(a => a.apellido)
                        .ThenBy(a => a.Nombres)
                        .ToListAsync();

                    ViewBag.Alumnos = new SelectList(
                        alumnos.Select(a => new
                        {
                            AlumnoID = a.AlumnoID,
                            NombreCompleto = $"{a.Nombres} {a.apellido} - {a.Carnet}"
                        }),
                        "AlumnoID",
                        "NombreCompleto"
                    );

                    return View();
                }

                var alumno = await _context.Alumnos
                    .FirstOrDefaultAsync(a => a.AlumnoID == alumnoId && a.Estado);

                if (alumno == null)
                {
                    TempData["ErrorMessage"] = "Alumno no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var notas = await _context.Notas
                    .Include(n => n.Asignacion)
                        .ThenInclude(a => a.Curso)
                    .Include(n => n.Asignacion)
                        .ThenInclude(a => a.Alumno)
                    .Where(n => n.Asignacion.AlumnoID == alumnoId && n.Estado)
                    .OrderByDescending(n => n.Asignacion.Anio)
                    .ThenBy(n => n.Asignacion.Ciclo)
                    .ThenBy(n => n.Asignacion.Curso.Nombre)
                    .ToListAsync();

                ViewBag.Alumno = alumno;
                return View(notas);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al generar la constancia: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}