using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversidadWEB.Controllers;
using UniversidadWEB.Models;


namespace UniversidadWEB.Controllers
{
    public class CatedraticosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatedraticosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Catedraticos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Catedraticos.ToListAsync());
        }

        // GET: Catedraticos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Catedraticos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatedraticoID,Nombres,Apellidos,Email,Telefono,Estado")] Catedratico catedratico)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el email ya existe
                if (await _context.Catedraticos.AnyAsync(c => c.Email == catedratico.Email))
                {
                    ModelState.AddModelError("Email", "Este correo electrónico ya está registrado");
                    return View(catedratico);
                }

                _context.Add(catedratico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(catedratico);
        }

        // GET: Catedraticos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catedratico = await _context.Catedraticos.FindAsync(id);
            if (catedratico == null)
            {
                return NotFound();
            }
            return View(catedratico);
        }

        // POST: Catedraticos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CatedraticoID,Nombres,Apellidos,Email,Telefono,Estado")] Catedratico catedratico)
        {
            if (id != catedratico.CatedraticoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el email ya existe en otro catedrático
                    if (await _context.Catedraticos.AnyAsync(c =>
                        c.Email == catedratico.Email &&
                        c.CatedraticoID != catedratico.CatedraticoID))
                    {
                        ModelState.AddModelError("Email", "Este correo electrónico ya está registrado");
                        return View(catedratico);
                    }

                    _context.Update(catedratico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatedraticoExists(catedratico.CatedraticoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(catedratico);
        }

        // GET: Catedraticos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catedratico = await _context.Catedraticos
                .FirstOrDefaultAsync(m => m.CatedraticoID == id);
            if (catedratico == null)
            {
                return NotFound();
            }

            return View(catedratico);
        }

        // POST: Catedraticos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var catedratico = await _context.Catedraticos.FindAsync(id);
            if (catedratico != null)
            {
                // Aquí podrías agregar una verificación para asegurarte de que el catedrático
                // no tiene cursos asignados antes de eliminarlo
                _context.Catedraticos.Remove(catedratico);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Catedraticos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catedratico = await _context.Catedraticos
                .FirstOrDefaultAsync(m => m.CatedraticoID == id);
            if (catedratico == null)
            {
                return NotFound();
            }

            return View(catedratico);
        }

        private bool CatedraticoExists(int id)
        {
            return _context.Catedraticos.Any(e => e.CatedraticoID == id);
        }
    }
}
