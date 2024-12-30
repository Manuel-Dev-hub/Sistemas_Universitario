using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversidadWEB.Models;

namespace UniversidadWEB.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cursos
        public async Task<IActionResult> Index()
        {
            try
            {
                var cursos = await _context.Cursos
                    .Include(c => c.Carrera)
                    .Where(c => c.Estado)
                    .OrderBy(c => c.Nombre)
                    .ToListAsync();
                return View(cursos);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los cursos: " + ex.Message;
                return View(new List<Curso>());
            }
        }

        // GET: Cursos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .Include(c => c.Carrera)
                .FirstOrDefaultAsync(m => m.CursoID == id && m.Estado);

            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // GET: Cursos/Create
        public IActionResult Create()
        {
            var curso = new Curso { Estado = true };
            PrepararCarrerasParaVista();
            return View(curso);
        }

        // POST: Cursos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CursoID,Nombre,Creditos,CarreraID,Estado")] Curso curso)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validar que no exista otro curso con el mismo nombre en la misma carrera
                    var cursoExistente = await _context.Cursos
                        .AnyAsync(c => c.Nombre.ToLower() == curso.Nombre.ToLower()
                                    && c.CarreraID == curso.CarreraID
                                    && c.Estado);

                    if (cursoExistente)
                    {
                        ModelState.AddModelError("Nombre", "Ya existe un curso con este nombre en la carrera seleccionada.");
                        PrepararCarrerasParaVista();
                        return View(curso);
                    }

                    // Asegurar que los créditos sean positivos
                    if (curso.Creditos <= 0) curso.Creditos = 1;
                    curso.Estado = true;

                    _context.Add(curso);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Curso creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el curso: " + ex.Message);
            }

            PrepararCarrerasParaVista();
            return View(curso);
        }

        // GET: Cursos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .Include(c => c.Carrera)
                .FirstOrDefaultAsync(c => c.CursoID == id && c.Estado);

            if (curso == null)
            {
                return NotFound();
            }

            PrepararCarrerasParaVista(curso.CarreraID);
            return View(curso);
        }

        // POST: Cursos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CursoID,Nombre,Creditos,CarreraID,Estado")] Curso curso)
        {
            if (id != curso.CursoID)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Validar que no exista otro curso con el mismo nombre en la misma carrera
                    var cursoExistente = await _context.Cursos
                        .AnyAsync(c => c.Nombre.ToLower() == curso.Nombre.ToLower()
                                    && c.CarreraID == curso.CarreraID
                                    && c.CursoID != curso.CursoID
                                    && c.Estado);

                    if (cursoExistente)
                    {
                        ModelState.AddModelError("Nombre", "Ya existe otro curso con este nombre en la carrera seleccionada.");
                        PrepararCarrerasParaVista(curso.CarreraID);
                        return View(curso);
                    }

                    // Asegurar que los créditos sean positivos
                    if (curso.Creditos <= 0) curso.Creditos = 1;
                    curso.Estado = true;

                    _context.Update(curso);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Curso actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CursoExists(curso.CursoID))
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
                ModelState.AddModelError("", "Error al actualizar el curso: " + ex.Message);
            }

            PrepararCarrerasParaVista(curso.CarreraID);
            return View(curso);
        }

        // GET: Cursos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .Include(c => c.Carrera)
                .FirstOrDefaultAsync(m => m.CursoID == id && m.Estado);

            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // POST: Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var curso = await _context.Cursos.FindAsync(id);
                if (curso != null)
                {
                    // Verificar si el curso tiene asignaciones
                    var tieneAsignaciones = await _context.Asignaciones
                        .AnyAsync(a => a.CursoID == id && a.Estado);

                    if (tieneAsignaciones)
                    {
                        TempData["ErrorMessage"] = "No se puede eliminar el curso porque tiene asignaciones activas.";
                        return RedirectToAction(nameof(Index));
                    }

                    curso.Estado = false; // Soft delete
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Curso eliminado exitosamente.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar el curso: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private void PrepararCarrerasParaVista(int? carreraID = null)
        {
            ViewData["CarreraID"] = new SelectList(
                _context.Carreras.Where(c => c.Estado).OrderBy(c => c.Nombre),
                "CarreraID",
                "Nombre",
                carreraID);
        }

        private bool CursoExists(int id)
        {
            return _context.Cursos.Any(e => e.CursoID == id && e.Estado);
        }
    }
}