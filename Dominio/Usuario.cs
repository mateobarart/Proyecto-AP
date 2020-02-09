using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        [Required]
        [DisplayName("Usuario")]
        public String NombreUsuario { get; set; }
        [Required]
        [DisplayName("Contraseña")]
        [DataType(DataType.Password)]
        public String Password { get; set; }
        [Required]
        [DisplayName("Tipo de usuario")]
        public TipoUsuario TipoUsuario { get; set; }
    }

    public enum TipoUsuario{ Analista, TeamLeader, Manager, Supervisor}
}