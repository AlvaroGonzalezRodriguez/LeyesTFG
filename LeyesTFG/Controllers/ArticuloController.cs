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
    public class ArticuloController : Controller
    {
        private readonly LeyesTFGContext _context;

        public ArticuloController(LeyesTFGContext context)
        {
            _context = context;
        }

        // GET: Articulo
        public async Task<IActionResult> Index(string busqueda)
        {
            ViewData["Filtro"] = busqueda;
            var articulo = _context.Articulo
                .Include(c => c.Ley)
                .AsNoTracking();
            if (!String.IsNullOrEmpty(busqueda))
            {
                articulo = articulo.Where(c => c.Ley.Titulo.Contains(busqueda));
            }

            return View(await articulo.ToListAsync());
        }

        // GET: Articulo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articulo = await _context.Articulo
                .Include(c => c.Modificaciones)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ArticuloId == id);
            if (articulo == null)
            {
                return NotFound();
            }

            return View(articulo);
        }

        // GET: Articulo/Create
        public IActionResult Create()
        {
            ListaDeLeyes();
            return View();
        }

        // POST: Articulo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticuloId,Titulo,Texto,LeyId")] Articulo articulo)
        {
            ModelState.Remove("Ley");
            ModelState.Remove("Modificaciones");
            if (ModelState.IsValid)
            {
                _context.Add(articulo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ListaDeLeyes(articulo.LeyId);
            return View(articulo);
        }

        // GET: Articulo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articulo = await _context.Articulo.FindAsync(id);
            if (articulo == null)
            {
                return NotFound();
            }
            ListaDeLeyes(articulo.LeyId);
            return View(articulo);
        }

        // POST: Articulo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticuloId,Titulo,Texto,LeyId")] Articulo articulo)
        {
            if (id != articulo.ArticuloId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(articulo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticuloExists(articulo.ArticuloId))
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
            ListaDeLeyes(articulo.LeyId);
            return View(articulo);
        }

        private void ListaDeLeyes(object LeySeleccionada = null)
        {
            var leyQuery = from l in _context.Ley
                           orderby l.LeyId
                           select l;
            ViewBag.LeyId = new SelectList(leyQuery.AsNoTracking(), "LeyId", "Titulo", LeySeleccionada);
        }

        // GET: Articulo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articulo = await _context.Articulo
                .FirstOrDefaultAsync(m => m.ArticuloId == id);
            if (articulo == null)
            {
                return NotFound();
            }

            return View(articulo);
        }

        // POST: Articulo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var articulo = await _context.Articulo.FindAsync(id);
            _context.Articulo.Remove(articulo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticuloExists(int id)
        {
            return _context.Articulo.Any(e => e.ArticuloId == id);
        }
    }
}
