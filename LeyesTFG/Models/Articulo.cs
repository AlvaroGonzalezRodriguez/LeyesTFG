using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeyesTFG.Models
{
    public class Articulo
    {
        public int ArticuloId { get; set; }

        [Required(ErrorMessage = "Debe de introducir un título")]
        [MinLength(3, ErrorMessage = "El título debe de tener un mínimo de 3 caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Debe de introducir algo de contenido al artículo")]
        public string Texto { get; set; }

        [Required(ErrorMessage = "Debe de introducir una ley asociada al artículo")]
        public int LeyId { get; set; }
        
        public Ley Ley { get; set; }

        public ICollection<Modificacion> Modificaciones { get; set; }
    }
}
