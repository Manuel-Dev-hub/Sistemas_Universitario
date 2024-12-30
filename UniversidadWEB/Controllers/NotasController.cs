using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversidadWEB.Models;
using UniversidadWEB;

public class NotasController : Controller
{
    private readonly ApplicationDbContext _context;

    public NotasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Notas
    public async Task<IActionResult> Index(string? busqueda, string? ciclo, int? anio)
    {
        var notas = _context.Notas
            .Include(n => n.Asignacion)
                .ThenInclude(a => a.Alumno)
            .Include(n => n.Asignacion)
                .ThenInclude(a => a.Curso)
            .Where(n => n.Estado)
            .AsQueryable();

        if (!string.IsNullOrEmpty(busqueda))
        {
            busqueda = busqueda.ToLower();
            notas = notas.Where(n =>
                n.Asignacion.Alumno.Nombres.ToLower().Contains(busqueda) ||
                n.Asignacion.Alumno.apellido.ToLower().Contains(busqueda) ||
                n.Asignacion.Curso.Nombre.ToLower().Contains(busqueda));
        }

        if (!string.IsNullOrEmpty(ciclo))
        {
            notas = notas.Where(n => n.Asignacion.Ciclo == ciclo);
        }

        if (anio.HasValue)
        {
            notas = notas.Where(n => n.Asignacion.Anio == anio);
        }

        ViewBag.Ciclos = await _context.Asignaciones
            .Where(a => a.Estado)
            .Select(a => a.Ciclo)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        ViewBag.Anios = await _context.Asignaciones
            .Where(a => a.Estado)
            .Select(a => a.Anio)
            .Distinct()
            .OrderByDescending(a => a)
            .ToListAsync();

        return View(await notas.OrderByDescending(n => n.FechaRegistro).ToListAsync());
    }

    // GET: Notas/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var nota = await _context.Notas
            .Include(n => n.Asignacion)
                .ThenInclude(a => a.Alumno)
            .Include(n => n.Asignacion)
                .ThenInclude(a => a.Curso)
            .FirstOrDefaultAsync(m => m.NotaID == id && m.Estado);

        if (nota == null)
        {
            return NotFound();
        }

        return View(nota);
    }

    // GET: Notas/Create
    public async Task<IActionResult> Create()
    {
        try
        {
            await CargarAsignaciones();
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al cargar las asignaciones: {ex.Message}");
            return View();
        }
    }

    // POST: Notas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AsignacionID,Calificacion")] Nota nota)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (!await ValidarAsignacion(nota.AsignacionID))
                {
                    ModelState.AddModelError("AsignacionID", "La asignación no es válida o ya tiene una nota registrada");
                    await CargarAsignaciones();
                    return View(nota);
                }

                nota.FechaRegistro = DateTime.Now;
                nota.Estado = true;

                _context.Add(nota);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Nota registrada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al guardar la nota: {ex.Message}");
        }

        await CargarAsignaciones();
        return View(nota);
    }

    // GET: Notas/Edit/5
    // GET: Notas/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var nota = await _context.Notas
                .Include(n => n.Asignacion)
                    .ThenInclude(a => a.Alumno)
                .Include(n => n.Asignacion)
                    .ThenInclude(a => a.Curso)
                .FirstOrDefaultAsync(n => n.NotaID == id && n.Estado);

            if (nota == null)
            {
                return NotFound();
            }

            if (nota.Asignacion == null)
            {
                return NotFound("No se encontró la asignación asociada a esta nota");
            }

            await CargarAsignacionesEditar(nota.AsignacionID);
            return View(nota);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al cargar la nota: {ex.Message}");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Notas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("NotaID,AsignacionID,Calificacion,FechaRegistro,Estado")] Nota nota)
    {
        if (id != nota.NotaID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var notaExistente = await _context.Notas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.NotaID == id);

                if (notaExistente == null)
                {
                    return NotFound();
                }

                if (!await ValidarAsignacion(nota.AsignacionID, nota.NotaID))
                {
                    ModelState.AddModelError("AsignacionID", "La asignación no es válida o ya tiene una nota registrada");
                    await CargarAsignacionesEditar(nota.AsignacionID);
                    return View(nota);
                }

                _context.Update(nota);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Nota actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotaExists(nota.NotaID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar la nota. Por favor, intente nuevamente.");
            }
        }

        await CargarAsignacionesEditar(nota.AsignacionID);
        return View(nota);
    }

    // GET: Notas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var nota = await _context.Notas
            .Include(n => n.Asignacion)
                .ThenInclude(a => a.Alumno)
            .Include(n => n.Asignacion)
                .ThenInclude(a => a.Curso)
            .FirstOrDefaultAsync(m => m.NotaID == id && m.Estado);

        if (nota == null)
        {
            return NotFound();
        }

        return View(nota);
    }

    // POST: Notas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var nota = await _context.Notas.FindAsync(id);
            if (nota != null)
            {
                nota.Estado = false;
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Nota eliminada exitosamente.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error al eliminar la nota.";
        }

        return RedirectToAction(nameof(Index));
    }

    private bool NotaExists(int id)
    {
        return _context.Notas.Any(e => e.NotaID == id);
    }

    private async Task<bool> ValidarAsignacion(int asignacionId, int? notaId = null)
    {
        try
        {
            // Verificar que la asignación existe y está activa
            var asignacionValida = await _context.Asignaciones
                .AnyAsync(a => a.AsignacionID == asignacionId &&
                              a.Estado);

            if (!asignacionValida)
            {
                return false;
            }

            // Verificar si ya existe una nota activa para esta asignación
            var notaExistente = await _context.Notas
                .AnyAsync(n => n.AsignacionID == asignacionId &&
                              n.Estado &&
                              (notaId == null || n.NotaID != notaId));

            return !notaExistente;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task CargarAsignaciones()
    {
        try
        {
            // Obtener asignaciones que no tienen notas activas
            var asignacionesDisponibles = await (
                from a in _context.Asignaciones
                where a.Estado &&
                      !_context.Notas.Any(n => n.AsignacionID == a.AsignacionID && n.Estado)
                select new SelectListItem
                {
                    Value = a.AsignacionID.ToString(),
                    Text = (a.Alumno.Nombres + " " + a.Alumno.apellido + " - " +
                           a.Curso.Nombre + " (" + a.Ciclo + " " + a.Anio + ")")
                })
                .OrderBy(x => x.Text)
                .ToListAsync();

            if (!asignacionesDisponibles.Any())
            {
                asignacionesDisponibles = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "",
                    Text = "-- No hay asignaciones disponibles --"
                }
            };
            }

            ViewBag.Asignaciones = asignacionesDisponibles;
        }
        catch (Exception ex)
        {
            ViewBag.Asignaciones = new List<SelectListItem>
        {
            new SelectListItem
            {
                Value = "",
                Text = "-- Error al cargar asignaciones --"
            }
        };
            throw new Exception($"Error al cargar asignaciones: {ex.Message}", ex);
        }
    }
    private async Task CargarAsignacionesEditar(int asignacionId)
    {
        try
        {
            // Obtener la asignación actual (la que estamos editando)
            var asignacionActual = await _context.Asignaciones
                .Include(a => a.Alumno)
                .Include(a => a.Curso)
                .FirstOrDefaultAsync(a => a.AsignacionID == asignacionId);

            if (asignacionActual == null)
            {
                throw new Exception("No se encontró la asignación");
            }

            // Obtener todas las asignaciones activas
            var todasLasAsignaciones = await _context.Asignaciones
                .Include(a => a.Alumno)
                .Include(a => a.Curso)
                .Where(a => a.Estado)
                .ToListAsync();

            var asignacionesLista = todasLasAsignaciones
                .Select(a => new
                {
                    a.AsignacionID,
                    Descripcion = string.Format("{0} {1} - {2} ({3} {4})",
                        a.Alumno?.Nombres ?? "",
                        a.Alumno?.apellido ?? "",
                        a.Curso?.Nombre ?? "",
                        a.Ciclo,
                        a.Anio)
                })
                .OrderBy(a => a.Descripcion)
                .ToList();

            ViewBag.Asignaciones = new SelectList(asignacionesLista, "AsignacionID", "Descripcion", asignacionId);
        }
        catch (Exception ex)
        {
            // En caso de error, crear una lista con al menos la asignación actual
            ViewBag.Asignaciones = new SelectList(
                new[] { new { AsignacionID = asignacionId, Descripcion = "Error al cargar asignaciones" } },
                "AsignacionID",
                "Descripcion",
                asignacionId
            );
            throw new Exception($"Error al cargar asignaciones para editar: {ex.Message}", ex);
        }
    }
}