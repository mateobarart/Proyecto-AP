using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        
        [Required]
        [DisplayName("Usuario")]
        public string NombreUsuario { get; set; }

        [Required]
        [DisplayName("Contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        [DisplayName("Tipo de usuario")]
        public TipoUsuario TipoUsuario { get; set; }

        [Required]
        [EmailAddress]
        //[Index(IsUnique = true)]
        public string Mail { get; set; }

        public int Puntaje { get; set; }

        public bool Activo { get; set; }

        public virtual List<Indisponibilidad> Indisponibilidades { get; set; }

        public Usuario()
        {
            Indisponibilidades = new List<Indisponibilidad>();
            Activo = true;
        }
    }

    public enum TipoUsuario{ Analista, TeamLeader, Manager, Supervisor}
}