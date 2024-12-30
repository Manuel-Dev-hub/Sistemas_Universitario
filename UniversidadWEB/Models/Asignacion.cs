using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversidadWEB.Models
{
    public class Asignacion
    {
        [Key]
        public int AsignacionID { get; set; }

        [Required(ErrorMessage = "El curso es requerido")]
        public int CursoID { get; set; }

        [Required(ErrorMessage = "El alumno es requerido")]
        public int AlumnoID { get; set; }

        [Required(ErrorMessage = "El catedrático es requerido")]
        public int CatedraticoID { get; set; }

        [Required(ErrorMessage = "El ciclo es requerido")]
        [StringLength(20)]
        public string Ciclo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año es requerido")]
        [Range(2000, 2100)]
        public int Anio { get; set; }

        [Required]
        public DateTime FechaAsignacion { get; set; }

        [Required]
        public bool Estado { get; set; }

        // Propiedades de navegación
        [ForeignKey("AlumnoID")]
        public virtual Alumno? Alumno { get; set; }

        [ForeignKey("CursoID")]
        public virtual Curso? Curso { get; set; }

        [ForeignKey("CatedraticoID")]
        public virtual Catedratico? Catedratico { get; set; }

        public Asignacion()
        {
            FechaAsignacion = DateTime.Now;
            Estado = true;
            Ciclo = string.Empty;
            Anio = DateTime.Now.Year;
        }
    }
}