using System.ComponentModel.DataAnnotations;
namespace UniversidadWEB.Models
{
    public class Carrera
    {
        public int CarreraID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255)]
        public string Descripcion { get; set; } = string.Empty;

        public bool Estado { get; set; }
    }
}