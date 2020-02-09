using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio
{
    [Table("Equipos")]
    public class Equipo
    {
        [Key]
        public int IdEquipo { get; set; }
        [Required]
        [DisplayName("Nombre del equipo")]
        public String NombreEquipo { get; set; }
        [Required]
        public List<Usuario> Analistas { get; set; }
    }
}