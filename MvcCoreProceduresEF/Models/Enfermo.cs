using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCoreProceduresEF.Models
{
    [Table("ENFERMO")]
    public class Enfermo
    {
        [Key]
        [Column("INSCRIPCION")]
        public string Inscripcion { get; set; }

        [Column("APELLIDO")]
        public string Apellido { get; set; }

        [Column("DIRECCION")]
        public string Direccion { get; set; }

        [Column("FECHA_NAC")]
        public DateTime FechaNac { get; set; }

        [Column("S")]
        public string Sexo { get; set; }

        [Column("NSS")]
        public string NSS { get; set; }
    }
}
