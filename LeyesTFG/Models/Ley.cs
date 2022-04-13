using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeyesTFG.Models
{
    public class Ley
    {
        public int LeyId { get; set; }

        [MinLength(3)]
        public string Titulo { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha publicación")]
        public DateTime? FechaPublicacion { get; set; }

        [MinLength(3)]
        public string Departamento { get; set; }

        public ICollection<Articulo> Articulos { get; set; }
    }
}
