using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeyesTFG.Models
{
    public class Articulo
    {
        public int ArticuloId { get; set; }

        [MinLength(3)]
        public string Titulo { get; set; }

        public string Texto { get; set; }

        public int LeyId { get; set; }
        
        public Ley Ley { get; set; }

        public ICollection<Modificacion> Modificaciones { get; set; }
    }
}
