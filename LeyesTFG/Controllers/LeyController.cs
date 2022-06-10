#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeyesTFG.Data;
using LeyesTFG.Models;

namespace LeyesTFG.Controllers
{
    public class LeyController : Controller
    {
        private readonly LeyesTFGContext _context;

        // Carga el contexto de la base de datos
        public LeyController(LeyesTFGContext context)
        {
            _context = context;
        }

        // GET: Ley
        // Carga los datos en la vista de indices en leyes, tambien añade el filtro
        public async Task<IActionResult> Index(string busqueda)
        {
            ViewData["Filtro"] = busqueda;
            var ley = _context.Ley
                .Include(c => c.Articulos)
                .AsNoTracking();

            if (!String.IsNullOrEmpty(busqueda))
            {
                ley = ley.Where(c => c.Titulo.Contains(busqueda));
            }

            return View(await ley.ToListAsync());
        }

        // GET: Ley/Details/5
        // Carga los datos de la ley en la pestaña de detalles
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ley = await _context.Ley
                .Include(c => c.Articulos)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.LeyId == id);
            if (ley == null)
            {
                return NotFound();
            }
            return View(ley);
        }

        // GET: Ley/Create
        // Inicia la vista de creación de leyes
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ley/Create
        // Guarda la ley nueva en la base de datos, tambien crea el mensaje de ley exitosa para las vistas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeyId,Titulo,FechaPublicacion,Departamento")] Ley ley)
        {
            ModelState.Remove("Articulos");
            if (ModelState.IsValid)
            {
                _context.Add(ley);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "¡Ley creada exitosamente!";
                return RedirectToAction(nameof(Index));
            }
            return View(ley);
        }

        // GET: Ley/Edit/5
        // Carga los datos de la ley a editar en la vista
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ley = await _context.Ley.FindAsync(id);
            if (ley == null)
            {
                return NotFound();
            }
            return View(ley);
        }

        // POST: Ley/Edit/5
        // Cambia en la base de datos la ley editada, además crea el mensaje de edicion exitosa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LeyId,Titulo,FechaPublicacion,Departamento")] Ley ley)
        {
            if (id != ley.LeyId)
            {
                return NotFound();
            }
            ModelState.Remove("Articulos");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ley);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "¡Ley editada exitosamente!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeyExists(ley.LeyId))
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
            return View(ley);
        }

        // GET: Ley/Delete/5
        // Carga los datos de la ley a eliminar en la vista
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ley = await _context.Ley
                .FirstOrDefaultAsync(m => m.LeyId == id);
            if (ley == null)
            {
                return NotFound();
            }

            return View(ley);
        }

        // POST: Ley/Delete/5
        // Borra la ley de la base de datos
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ley = await _context.Ley.FindAsync(id);
            _context.Ley.Remove(ley);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "¡Ley eliminada exitosamente!";
            return RedirectToAction(nameof(Index));
        }

        private bool LeyExists(int id)
        {
            return _context.Ley.Any(e => e.LeyId == id);
        }
    }
}
