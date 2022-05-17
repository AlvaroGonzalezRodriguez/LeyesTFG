using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeyesTFG.Models
{
    public class Ley
    {
        public int LeyId { get; set; }

        [Required(ErrorMessage = "Debe de introducir un título")]
        [MinLength(3, ErrorMessage = "El título debe de tener un mínimo de 3 caracteres")]
        public string Titulo { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha publicación")]
        public DateTime? FechaPublicacion { get; set; }

        [Required(ErrorMessage = "Debe de introducir un departamento")]
        [MinLength(3, ErrorMessage = "El departamento debe de tener un mínimo de 3 caracteres")]
        public string Departamento { get; set; }

        public ICollection<Articulo> Articulos { get; set; }
    }
}
