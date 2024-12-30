using System.ComponentModel.DataAnnotations;

namespace UniversidadWEB.Models
{
    public class Curso
    {
        public int CursoID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los créditos son obligatorios")]
        [Range(1, 10, ErrorMessage = "Los créditos deben estar entre 1 y 10")]
        [Display(Name = "Créditos")]
        public int Creditos { get; set; } = 1;

        [Required(ErrorMessage = "La carrera es obligatoria")]
        [Display(Name = "Carrera")]
        public int CarreraID { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        // Propiedad de navegación
        public virtual Carrera? Carrera { get; set; }
    }
}