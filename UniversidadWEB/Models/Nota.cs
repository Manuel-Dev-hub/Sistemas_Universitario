// Models/Nota.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversidadWEB.Models
{
    public class Nota
    {
        [Key]
        public int NotaID { get; set; }

        [Required(ErrorMessage = "La asignación es requerida")]
        public int AsignacionID { get; set; }

        [Required(ErrorMessage = "La calificación es requerida")]
        [Range(0, 100, ErrorMessage = "La calificación debe estar entre 0 y 100")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Calificacion { get; set; }

        [Required]
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; }

        public bool Estado { get; set; }

        [ForeignKey("AsignacionID")]
        public virtual Asignacion? Asignacion { get; set; }
    }
}