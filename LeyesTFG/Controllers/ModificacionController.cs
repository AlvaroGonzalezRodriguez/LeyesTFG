#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
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

            DiferenciaTexto(id, modificacion);
            return View(modificacion);
        }

        // GET: Modificacion/Create
        public IActionResult Create(int? id)
        {
            Modificacion modificacion = null;
            if (id != null)
            {
                modificacion = RecogerDatosArticulo(id);
            } else
            {
                modificacion = new Modificacion();
                modificacion.Texto = "<p>Introduzca <strong>aquí</strong> el contenido del artículo</p> <p><br></p>";
            }
            ListaDeArticulos();
            return View(modificacion);
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

        private Modificacion RecogerDatosArticulo(int? id)
        {
            var articuloTexto = from a in _context.Articulo
                           where a.ArticuloId == id
                           orderby a.ArticuloId
                           select a.Texto;

            var articuloId = from a in _context.Articulo
                                where a.ArticuloId == id
                                orderby a.ArticuloId
                                select a.ArticuloId;

            Modificacion modificacion = new Modificacion();
            modificacion.ArticuloId = articuloId.First();
            modificacion.Texto = articuloTexto.First();

            return modificacion;
        }

        public enum CharState {Add,Equal,Remove}

        public struct CharResult
        {
            public char c;
            public CharState state;
        }

        private void DiferenciaTexto(int? id, Modificacion modificacion)
        {
            var textoOriginal = from b in _context.Articulo
                                where b.ArticuloId == (from c in _context.Modificacion where c.ArticuloId == b.ArticuloId && c.ModificacionId == id select c.ArticuloId).Single()
                                orderby b.ArticuloId
                                select b.Texto;

            var textoMod = from a in _context.Modificacion
                           where a.ModificacionId == id
                           orderby a.ModificacionId
                           select a.Texto;
                           
            string[] texto1 = textoOriginal.ToArray();
            string[] texto2 = textoMod.ToArray();
            IEnumerable<String> textoAgregado = null;
            IEnumerable<String> textoQuitado = null;

            texto1[0] = QuitarTagsHTML(texto1[0]); 
            texto2[0] = QuitarTagsHTML(texto2[0]);
            string[] palabras1 = texto1[0].Split();
            string[] palabras2 = texto2[0].Split();

            List<CharResult> textoMarcado = EditSequenceLevensthein(texto1[0], texto2[0]);

            textoAgregado = palabras2.Except(palabras1);
            textoQuitado = palabras1.Except(palabras2);

            ViewBag.TextoQuitado = textoQuitado;
            ViewBag.TextoAgregado = textoAgregado;
            ViewBag.TextoMarcado = textoMarcado;
        }

        private static List<CharResult> EditSequenceLevensthein(string source, string target, int insertCost = 1, int removeCost = 1, int editCost = 2)
        {
            if (null == source)
                throw new ArgumentNullException("source");
            else if (null == target)
                throw new ArgumentNullException("target");
            // Forward: building score matrix
            // Best operation (among insert, update, delete) to perform 
            CharState[][] M = Enumerable.Range(0, source.Length + 1).Select(line => new CharState[target.Length + 1]).ToArray();
            // Minimum cost so far
            int[][] D = Enumerable.Range(0, source.Length + 1).Select(line => new int[target.Length + 1]).ToArray();
            // Edge: all removes
            for (int i = 1; i <= source.Length; ++i)
            {
                M[i][0] = CharState.Remove;
                D[i][0] = removeCost * i;
            }

            // Edge: all inserts 
            for (int i = 1; i <= target.Length; ++i)
            {
                M[0][i] = CharState.Add;
                D[0][i] = insertCost * i;
            }

            // Having fit N - 1, K - 1 characters let's fit N, K
            for (int i = 1; i <= source.Length; ++i)
                for (int j = 1; j <= target.Length; ++j)
                {
                    // here we choose the operation with the least cost
                    int insert = D[i][j - 1] + insertCost;
                    int delete = D[i - 1][j] + removeCost;
                    int edit = D[i - 1][j - 1] + (source[i - 1] == target[j - 1] ? 0 : editCost);
                    int min = Math.Min(Math.Min(insert, delete), edit);
                    if (min == insert)
                        M[i][j] = CharState.Add;
                    else if (min == delete)
                        M[i][j] = CharState.Remove;
                    else if (min == edit)
                        M[i][j] = CharState.Equal;
                    D[i][j] = min;
                }

            // Backward: knowing scores (D) and actions (M) let's building edit sequence
            List<CharResult> result = new List<CharResult>(Math.Max(source.Length, target.Length));
            for (int x = target.Length, y = source.Length; (x > 0) || (y > 0);)
            {
                CharState op = M[y][x];
                if (op == CharState.Add)
                {
                    x -= 1;
                    result.Add(new CharResult()
                    { state = CharState.Add, c = target[x] });
                }
                else if (op == CharState.Remove)
                {
                    y -= 1;
                    result.Add(new CharResult()
                    { c = source[y], state = CharState.Remove });
                }
                else if (op == CharState.Equal)
                {
                    x -= 1;
                    y -= 1;
                    if (source[y] == target[x])
                    {
                        result.Add(new CharResult()
                        { state = CharState.Equal, c = target[x] });
                    }
                    else
                    {
                        result.Add(new CharResult()
                        { state = CharState.Add, c = target[x] });
                        result.Add(new CharResult()
                        { state = CharState.Remove, c = source[y] });
                    }
                }
                else // <- Start
                    break;
            }
            result.Reverse();
            return result;
        }

        private static string QuitarTagsHTML(string texto)
        {
            char[] array = new char[texto.Length];
            int arrayIndex = 0;
            bool dentro = false;
            for (int i = 0; i < texto.Length; i++)
            {
                char let = texto[i];
                if (let == '<')
                {
                    dentro = true;
                    continue;
                }
                if (let == '>')
                {
                    dentro = false;
                    continue;
                }
                if (!dentro)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
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

        public async Task<IActionResult> Accept(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            return RedirectToAction(nameof(Index));
        }

        private bool ModificacionExists(int id)
        {
            return _context.Modificacion.Any(e => e.ModificacionId == id);
        }
    }
}
