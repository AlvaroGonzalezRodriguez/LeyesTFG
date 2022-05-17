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
    public class ModificacionController : Controller
    {
        private readonly LeyesTFGContext _context;

        public ModificacionController(LeyesTFGContext context)
        {
            _context = context;
        }

        // GET: Modificacion
        public async Task<IActionResult> Index()
        {
            var modificacion = _context.Modificacion
                .Include(c => c.Articulo)
                .AsNoTracking();
            return View(await modificacion.ToListAsync());
        }

        // GET: Modificacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modificacion = await _context.Modificacion
                .Include(c => c.Articulo)
                .FirstOrDefaultAsync(m => m.ModificacionId == id);
            if (modificacion == null)
            {
                return NotFound();
            }

            return View(modificacion);
        }

        // GET: Modificacion/Create
        public IActionResult Create()
        {
            ListaDeArticulos();
            return View();
        }


        // POST: Modificacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModificacionId,Titulo,Texto,ArticuloId")] Modificacion modificacion)
        {
            ModelState.Remove("Articulo");
            if (ModelState.IsValid)
            {
                _context.Add(modificacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ListaDeArticulos(modificacion.ArticuloId);
            return View(modificacion);
        }

        // GET: Modificacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modificacion = await _context.Modificacion.FindAsync(id);
            if (modificacion == null)
            {
                return NotFound();
            }
            ListaDeArticulos(modificacion.ArticuloId);
            return View(modificacion);
        }

        // POST: Modificacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ModificacionId,Titulo,Texto,ArticuloId")] Modificacion modificacion)
        {
            if (id != modificacion.ModificacionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modificacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModificacionExists(modificacion.ModificacionId))
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
            ListaDeArticulos(modificacion.ArticuloId);
            return View(modificacion);
        }

        private void ListaDeArticulos(object ArticuloSeleccionado = null)
        {
            var articuloQuery = from a in _context.Articulo
                                orderby a.ArticuloId
                                select a;
            ViewBag.ArticuloId = new SelectList(articuloQuery.AsNoTracking(), "ArticuloId", "Titulo", ArticuloSeleccionado);
        }

        

        // GET: Modificacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modificacion = await _context.Modificacion
                .FirstOrDefaultAsync(m => m.ModificacionId == id);
            if (modificacion == null)
            {
                return NotFound();
            }

            return View(modificacion);
        }

        // POST: Modificacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modificacion = await _context.Modificacion.FindAsync(id);
            _context.Modificacion.Remove(modificacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModificacionExists(int id)
        {
            return _context.Modificacion.Any(e => e.ModificacionId == id);
        }
    }
}
