using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    [Table("Equipos")]
    public class Equipo
    {
        
        public Equipo() { }
        public Equipo(string nombreEquipo, Usuario titular, Usuario suplente, Usuario reserva)
        {
            NombreEquipo = nombreEquipo;
            Titular = titular;
            Suplente = suplente;
            Reserva = reserva;
        }

        [Key]
        public int IdEquipo { get; set; }

        [Required]
        [StringLength(450)]
        [Index(IsUnique = true)]
        [DisplayName("Nombre del equipo")]
        public string NombreEquipo { get; set; }

        /*[Required]
        public List<Usuario> Analistas { get; set; }*/

        [DisplayName("Analista titular")]
        public virtual Usuario Titular { get; set; }

        [DisplayName("Analista suplente")]
        public virtual Usuario Suplente { get; set; }

        [DisplayName("Analista reserva")]
        public virtual Usuario Reserva { get; set; }

    }
}