using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeyesTFG.Models
{
    public class Modificacion
    {
        public int ModificacionId { get; set; }

        [Required(ErrorMessage = "Debe de introducir un título")]
        [MinLength(3, ErrorMessage = "El título debe de tener un mínimo de 3 caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Debe de introducir algo de contenido a la modificación")]
        public string Texto { get; set; }

        [Required(ErrorMessage = "Debe de introducir un artículo asociado a la modificación")]
        public int ArticuloId { get; set; }

        public bool Aceptado { get; set; }

        public Articulo Articulo { get; set; }
    }
}
