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

            List<Articulo> listaArt = articulo.ToList();
            List<string> textoTruncado = new List<string>();
            for (int i = 0; i < listaArt.Count; i++)
            {
                textoTruncado.Add(ModificacionController.Truncar(listaArt.ElementAt(i).Texto, 500));
            }
            ViewBag.truncar = textoTruncado;

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
                .Include(a => a.Ley)
                .Include(c => c.Modificaciones)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ArticuloId == id);
            if (articulo == null)
            {
                return NotFound();
            }

            List<CharResult> diferencia = new List<CharResult>();
            if (!articulo.TextoAnterior.Equals("Este articulo no ha sido modificado nunca"))
            {
                string dummy1 = articulo.Texto;
                string dummy2 = articulo.TextoAnterior;
                dummy1 = ModificacionController.QuitarTagsHTML(dummy1);
                dummy2 = ModificacionController.QuitarTagsHTML(dummy2);
                diferencia = ModificacionController.EditSequenceLevensthein(dummy2, dummy1);
            }
            ViewBag.Diferencia = diferencia;

            var modAceptados = from a in _context.Modificacion
                               where a.ArticuloId == articulo.ArticuloId
                               select a.Aceptado;
            var modEvaluado = from a in _context.Modificacion
                              where a.ArticuloId == articulo.ArticuloId
                              select a.PendienteEva;
            List<bool> listaAceptado = modAceptados.ToList();
            List<bool> listaEvaluado = modEvaluado.ToList();
            List<string> aceptados = new List<string>();

            for (int i = 0; i < listaAceptado.Count; i++)
            {
                if (listaEvaluado[i])
                {
                    if (listaAceptado[i])
                    {
                        aceptados.Add("ACEPTADO");
                    }
                    else
                    {
                        aceptados.Add("NO ACEPTADO");
                    }
                } else
                {
                    aceptados.Add("PENDIENTE A EVALUAR");
                }
                
            }
            ViewBag.Aceptado = aceptados;

            return View(articulo);
        }

        // GET: Articulo/Create
        public IActionResult Create()
        {
            ListaDeLeyes();
            return View();
        }

        // POST: Articulo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticuloId,Titulo,Texto,LeyId")] Articulo articulo)
        {
            ModelState.Remove("Ley");
            ModelState.Remove("Modificaciones");
            if (ModelState.IsValid)
            {
                articulo.TextoAnterior = "Este articulo no ha sido modificado nunca";
                _context.Add(articulo);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "¡Artículo creado exitosamente!";
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
                    TempData["Mensaje"] = "¡Artículo editado exitosamente!";
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
            TempData["Mensaje"] = "¡Artículo borrado exitosamente!";
            return RedirectToAction(nameof(Index));
        }

        private bool ArticuloExists(int id)
        {
            return _context.Articulo.Any(e => e.ArticuloId == id);
        }
    }
}
