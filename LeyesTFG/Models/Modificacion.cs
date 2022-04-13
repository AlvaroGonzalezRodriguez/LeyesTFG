using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeyesTFG.Models
{
    public class Modificacion
    {
        public int ModificacionId { get; set; }

        [MinLength(3)]
        public string Titulo { get; set; }
        public string Texto { get; set; }

        public int ArticuloId { get; set; }

        public Articulo Articulo { get; set; }
    }
}
