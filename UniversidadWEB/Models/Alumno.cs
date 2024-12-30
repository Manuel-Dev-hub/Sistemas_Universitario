using System.ComponentModel.DataAnnotations;

namespace UniversidadWEB.Models
{
    public class Alumno
    {
        public int AlumnoID { get; set; }

        [Required(ErrorMessage = "El carnet es obligatorio")]
        [StringLength(20)]
        [Display(Name = "Carnet")]
        public string Carnet { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Apellidos")]
        public string apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto => $"{Nombres} {apellido}";
    }
}