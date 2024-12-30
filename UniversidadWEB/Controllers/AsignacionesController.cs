using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversidadWEB.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UniversidadWEB.Controllers
{
    public class AsignacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AsignacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Asignaciones
        public async Task<IActionResult> Index(string busqueda, string ciclo, int? anio)
        {
            try
            {
                var asignaciones = _context.Asignaciones
                    .Include(a => a.Alumno)
                    .Include(a => a.Curso)
                    .Include(a => a.Catedratico)
                    .Where(a => a.Estado)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(busqueda))
                {
                    busqueda = busqueda.ToLower();
                    asignaciones = asignaciones.Where(a =>
                        a.Alumno.Nombres.ToLower().Contains(busqueda) ||
                        a.Alumno.apellido.ToLower().Contains(busqueda) ||
                        a.Curso.Nombre.ToLower().Contains(busqueda));
                }

                if (!string.IsNullOrEmpty(ciclo))
                {
                    asignaciones = asignaciones.Where(a => a.Ciclo == ciclo);
                }

                if (anio.HasValue)
                {
                    asignaciones = asignaciones.Where(a => a.Anio == anio);
                }

                ViewData["Ciclos"] = await _context.Asignaciones
                    .Where(a => a.Estado)
                    .Select(a => a.Ciclo)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

                ViewData["Anios"] = await _context.Asignaciones
                    .Where(a => a.Estado)
                    .Select(a => a.Anio)
                    .Distinct()
                    .OrderByDescending(a => a)
                    .ToListAsync();

                return View(await asignaciones.OrderByDescending(a => a.FechaAsignacion).ToListAsync());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar las asignaciones: " + ex.Message;
                return View(new List<Asignacion>());
            }
        }

        // GET: Asignaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignacion = await _context.Asignaciones
                .Include(a => a.Alumno)
                .Include(a => a.Curso)
                .Include(a => a.Catedratico)
                .FirstOrDefaultAsync(m => m.AsignacionID == id && m.Estado);

            if (asignacion == null)
            {
                return NotFound();
            }

            return View(asignacion);
        }

        // GET: Asignaciones/Create
        public async Task<IActionResult> Create()
        {
            await CargarViewBagData();
            return View();
        }

        // POST: Asignaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlumnoID,CursoID,CatedraticoID,Ciclo,Anio")] Asignacion asignacion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar si ya existe una asignación similar
                    var existeAsignacion = await _context.Asignaciones
                        .AnyAsync(a => a.AlumnoID == asignacion.AlumnoID
                            && a.CursoID == asignacion.CursoID
                            && a.Ciclo == asignacion.Ciclo
                            && a.Anio == asignacion.Anio
                            && a.Estado);

                    if (existeAsignacion)
                    {
                        ModelState.AddModelError("", "Ya existe una asignación para este alumno en este curso, ciclo y año");
                        await CargarViewBagData();
                        return View(asignacion);
                    }

                    // Establecer valores por defecto
                    asignacion.Estado = true;
                    asignacion.FechaAsignacion = DateTime.Now;

                    _context.Add(asignacion);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Asignación creada con éxito";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error de validación: {error.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear la asignación: {ex.Message}");
                Console.WriteLine($"Error completo: {ex}");
            }

            await CargarViewBagData();
            return View(asignacion);
        }

        // GET: Asignaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignacion = await _context.Asignaciones
                .Include(a => a.Alumno)
                .Include(a => a.Curso)
                .Include(a => a.Catedratico)
                .FirstOrDefaultAsync(a => a.AsignacionID == id && a.Estado);

            if (asignacion == null)
            {
                return NotFound();
            }

            await CargarViewBagData();
            return View(asignacion);
        }

        // POST: Asignaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Asignacion asignacion)
        {
            if (id != asignacion.AsignacionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var asignacionExistente = await _context.Asignaciones
                        .AnyAsync(a => a.AlumnoID == asignacion.AlumnoID
                            && a.CursoID == asignacion.CursoID
                            && a.Ciclo == asignacion.Ciclo
                            && a.Anio == asignacion.Anio
                            && a.AsignacionID != id
                            && a.Estado);

                    if (asignacionExistente)
                    {
                        ModelState.AddModelError("", "Ya existe una asignación para este alumno en este curso, ciclo y año.");
                        await CargarViewBagData();
                        return View(asignacion);
                    }

                    var asignacionOriginal = await _context.Asignaciones.FindAsync(id);
                    if (asignacionOriginal == null)
                    {
                        return NotFound();
                    }

                    asignacionOriginal.AlumnoID = asignacion.AlumnoID;
                    asignacionOriginal.CursoID = asignacion.CursoID;
                    asignacionOriginal.CatedraticoID = asignacion.CatedraticoID;
                    asignacionOriginal.Ciclo = asignacion.Ciclo;
                    asignacionOriginal.Anio = asignacion.Anio;

                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Asignación actualizada con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar la asignación: " + ex.Message);
                }
            }

            await CargarViewBagData();
            return View(asignacion);
        }

        // GET: Asignaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignacion = await _context.Asignaciones
                .Include(a => a.Alumno)
                .Include(a => a.Curso)
                .Include(a => a.Catedratico)
                .FirstOrDefaultAsync(m => m.AsignacionID == id && m.Estado);

            if (asignacion == null)
            {
                return NotFound();
            }

            return View(asignacion);
        }

        // POST: Asignaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var asignacion = await _context.Asignaciones.FindAsync(id);
                if (asignacion != null)
                {
                    asignacion.Estado = false;
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Asignación eliminada con éxito.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la asignación: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task CargarViewBagData()
        {
            try
            {
                var alumnos = await _context.Alumnos
                    .Where(a => a.Estado)
                    .Select(a => new SelectListItem
                    {
                        Value = a.AlumnoID.ToString(),
                        Text = $"{a.Nombres} {a.apellido}"
                    })
                    .ToListAsync();
                ViewData["Alumnos"] = alumnos;

                var cursos = await _context.Cursos
                    .Where(c => c.Estado)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CursoID.ToString(),
                        Text = c.Nombre
                    })
                    .ToListAsync();
                ViewData["Cursos"] = cursos;

                var catedraticos = await _context.Catedraticos
                    .Where(c => c.Estado)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CatedraticoID.ToString(),
                        Text = $"{c.Nombres} {c.Apellidos}"
                    })
                    .ToListAsync();
                ViewData["Catedraticos"] = catedraticos;

                ViewData["Ciclos"] = new List<string> { "1", "2", "3", "4", "5" };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar los datos: {ex.Message}");
            }
        }

        private bool AsignacionExists(int id)
        {
            return _context.Asignaciones.Any(e => e.AsignacionID == id);
        }
    }
}