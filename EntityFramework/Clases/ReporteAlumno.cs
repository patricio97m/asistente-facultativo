using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Clases
{
    public class ReporteAlumno
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int MateriaID { get; set; }
        public int? AlumnoID { get; set; }
        public int NotaFinal { get; set; }
        public bool Promocionable { get; set; }

        [ForeignKey("MateriaID")]
        public virtual Materia ?Materia { get; set; }
        [ForeignKey("AlumnoID")]
        public virtual Alumno ?Alumno { get; set; }
    }
}
