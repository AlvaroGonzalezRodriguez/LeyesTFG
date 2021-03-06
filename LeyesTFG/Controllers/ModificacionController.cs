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

        // Carga el contexto de la base de datos
        public ModificacionController(LeyesTFGContext context)
        {
            _context = context;
        }

        // GET: Modificacion
        // Carga los datos en la vista de indices en modificaciones, tambien añade el filtro, el estado y trunca el texto
        public async Task<IActionResult> Index(string busqueda)
        {
            ViewData["Filtro"] = busqueda;

            var modificacion = _context.Modificacion
                .Include(c => c.Articulo)
                .AsNoTracking();
            if (!String.IsNullOrEmpty(busqueda))
            {
                modificacion = modificacion.Where(c => c.Titulo.Contains(busqueda));
            }
            List<Modificacion> listaMod = modificacion.ToList();
            List<string> estado = new List<string>();
            List<string> textoTruncado = new List<string>();
            for (int i = 0; i < listaMod.Count; i++)
            {
                if(listaMod.ElementAt(i).PendienteEva)
                {
                    if (listaMod.ElementAt(i).Aceptado)
                    {
                        estado.Add("Add");
                    } else
                    {
                        estado.Add("Remove");
                    }
                } else
                {
                    estado.Add("back_ora");
                }
                textoTruncado.Add(Truncar(listaMod.ElementAt(i).Texto, 500));
            }
            ViewBag.estado = estado;
            ViewBag.truncar = textoTruncado;

            return View(await modificacion.ToListAsync());
        }

        // GET: Modificacion/Details/5
        // Carga los datos de la modificación en la pestaña de detalles
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modificacion = await _context.Modificacion
                .Include(c => c.Articulo)
                    .ThenInclude(a => a.Ley)
                .FirstOrDefaultAsync(m => m.ModificacionId == id);

            if (modificacion == null)
            {
                return NotFound();
            }
            string estado;
            if (modificacion.PendienteEva)
            {
                if (modificacion.Aceptado)
                {
                    estado = "ACEPTADO";
                } else
                {
                    estado = "NO ACEPTADO";
                }
            } else
            {
                estado = "PENDIENTE A EVALUAR";
            }
            ViewBag.Estado = estado;
            DiferenciaTexto(id, modificacion);
            return View(modificacion);
        }

        // GET: Modificacion/Create
        // Carga la nueva modificación en la pestaña de creación, si tiene una id significa que se tiene que autocompletar los datos con un articulo
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
                modificacion.PendienteEva = true;
            }
            ListaDeArticulos();
            return View(modificacion);
        }


        // POST: Modificacion/Create
        // Guarda la modificacion nueva en la base de datos, tambien crea el mensaje de modificacion exitosa para las vistas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModificacionId,Titulo,Texto,ArticuloId")] Modificacion modificacion)
        {
            ModelState.Remove("Articulo");
            if (ModelState.IsValid)
            {
                _context.Add(modificacion);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "¡Modificación creada exitosamente!";
                return RedirectToAction(nameof(Index));
            }
            ListaDeArticulos(modificacion.ArticuloId);
            return View(modificacion);
        }

        // GET: Modificacion/Edit/5
        // Carga los datos de la modificacion a editar en la vista
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
        // Cambia en la base de datos la modificación editada, además crea el mensaje de edicion exitosa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ModificacionId,Titulo,Texto,ArticuloId,PendienteEva,Aceptado")] Modificacion modificacion)
        {
            if (id != modificacion.ModificacionId)
            {
                return NotFound();
            }
            ModelState.Remove("Articulo");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modificacion);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "¡Modificación editada exitosamente!";
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

        // Crea una lista de articulos para usar en las vistas
        private void ListaDeArticulos(object ArticuloSeleccionado = null)
        {
            var articuloQuery = from a in _context.Articulo
                                orderby a.ArticuloId
                                select a;
            ViewBag.ArticuloId = new SelectList(articuloQuery.AsNoTracking(), "ArticuloId", "Titulo", ArticuloSeleccionado);
        }

        // Recoge los datos de un articulo y se lo asigna a una nueva modificacion, usada para autocompletar texto
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

        // Consulta el texto de un articulo para comparar las diferencias con una modificacion
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

            texto1[0] = QuitarTagsHTML(texto1[0]); 
            texto2[0] = QuitarTagsHTML(texto2[0]);

            List<CharResult> textoMarcado = EditSequenceLevensthein(texto1[0], texto2[0]);

            ViewBag.TextoMarcado = textoMarcado;
        }

        // La diferencia en el texto se realiza haciendo uso de la secuencia de levensthein
        public static List<CharResult> EditSequenceLevensthein(string source, string target, int insertCost = 1, int removeCost = 1, int editCost = 2)
        {
            if (null == source)
                throw new ArgumentNullException("source");
            else if (null == target)
                throw new ArgumentNullException("target");
            // Recoge la mejor operación
            CharState[][] M = Enumerable.Range(0, source.Length + 1).Select(line => new CharState[target.Length + 1]).ToArray();
            // Coste mínimo
            int[][] D = Enumerable.Range(0, source.Length + 1).Select(line => new int[target.Length + 1]).ToArray();
            for (int i = 1; i <= source.Length; ++i)
            {
                M[i][0] = CharState.Remove;
                D[i][0] = removeCost * i;
            }

            for (int i = 1; i <= target.Length; ++i)
            {
                M[0][i] = CharState.Add;
                D[0][i] = insertCost * i;
            }

            for (int i = 1; i <= source.Length; ++i)
                for (int j = 1; j <= target.Length; ++j)
                {
                    // Selecciona la operación con menor coste
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

            // Construye la secuencia con los valores de D y M
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
                else
                    break;
            }
            result.Reverse();
            return result;
        }

        // Trunca el texto con una determinada longitud
        public static string Truncar(string valor, int longitud)
        {
            if(valor.Length > longitud)
            {
                return valor.Substring(0, longitud) + "...";
            } else
            {
                return valor;
            }
        }

        // Quita las etiquetas html de una cadena
        public static string QuitarTagsHTML(string texto)
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
        // Carga los datos de la modificacion a eliminar en la vista
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
        // Borra la modificación de la base de datos
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modificacion = await _context.Modificacion.FindAsync(id);
            _context.Modificacion.Remove(modificacion);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "¡Modificación eliminada exitosamente!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Modificacion/Accept/5
        // Carga los datos de la modificación a aceptar en la vista
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

            string textoMod = QuitarTagsHTML(modificacion.Texto);
            string textoOrig = QuitarTagsHTML(modificacion.Articulo.Texto);

            List<CharResult> textoMarcado = EditSequenceLevensthein(textoOrig, textoMod);

            ViewBag.TextoMarcado = textoMarcado;

            return View(modificacion);
        }

        // POST: Modificacion/Accept/5
        // Modifica los datos de la modificacion para que sea aceptada y tambien cambia los datos de texto anterior del articulo modificado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var modificacion = await _context.Modificacion.FindAsync(id);
            modificacion.Aceptado = true;
            modificacion.PendienteEva = true;

            var allMod = from a in _context.Modificacion
                         where a.ModificacionId != id && a.ArticuloId == modificacion.ArticuloId
                         select a;
            List<Modificacion> listaMod = allMod.ToList();
            //UNA VEZ SE ACEPTA UNA MODIFICACION EL RESTO SE DENIEGAN AL MOMENTO
            for (int i = 0; i < listaMod.Count; i++)
            {
                listaMod.ElementAt(i).PendienteEva = true;
                listaMod.ElementAt(i).Aceptado = false;
                _context.Update(listaMod.ElementAt(i));
            }
            // Busca el articulo que tiene que cambiar el texto
            var articulo = await _context.Articulo.FirstOrDefaultAsync(item => item.ArticuloId == modificacion.ArticuloId);
            articulo.TextoAnterior = articulo.Texto;
            articulo.Texto = modificacion.Texto;
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "¡Modificación aceptada exitosamente!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Modificacion/Accept/5
        // Cambia los datos de la modificacion para que esta sea rechazada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Denegate(int id)
        {
            var modificacion = await _context.Modificacion.FindAsync(id);
            modificacion.Aceptado = false;
            modificacion.PendienteEva = true;
            _context.Update(modificacion);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "¡Modificación denegada exitosamente!";
            return RedirectToAction(nameof(Index));
        }

        private bool ModificacionExists(int id)
        {
            return _context.Modificacion.Any(e => e.ModificacionId == id);
        }
    }
}
