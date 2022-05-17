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

        public LeyController(LeyesTFGContext context)
        {
            _context = context;
        }

        // GET: Ley
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
            //ListaDeArticulos();
            return View(ley);
        }

        // GET: Ley/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ley/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeyId,Titulo,FechaPublicacion,Departamento")] Ley ley)
        {
            ModelState.Remove("Articulos");
            if (ModelState.IsValid)
            {
                _context.Add(ley);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ley);
        }

        // GET: Ley/Edit/5
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LeyId,Titulo,FechaPublicacion,Departamento")] Ley ley)
        {
            if (id != ley.LeyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ley);
                    await _context.SaveChangesAsync();
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
        /*private void ListaDeArticulos()
        {
            var articulosQuery = from a in _context.Articulo
                                 orderby a.LeyId
                                 select a;
            ViewBag.ListaArticulos = new SelectList(articulosQuery.AsNoTracking(), "ArticuloId", "Titulo");
        }*/

        // GET: Ley/Delete/5
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ley = await _context.Ley.FindAsync(id);
            _context.Ley.Remove(ley);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeyExists(int id)
        {
            return _context.Ley.Any(e => e.LeyId == id);
        }
    }
}
