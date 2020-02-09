using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    [Table("IndisponibilidadRecurrente")]
    public class IndisponibilidadRecurrente:Indisponibilidad
    {
        [Required]
        [DisplayName("Día semana")]
        public string DiaSemana { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [Required]
        [DisplayName("Hora Inicio")]
        public string HoraInicio { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [Required]
        [DisplayName("Hora Fin")]
        public string HoraFin { get; set; }

        public IndisponibilidadRecurrente(): base()
        {

        }
        public IndisponibilidadRecurrente(string diaSemana, Usuario usuario, string horaInicio, string horaFin): base (usuario)
        {
            this.DiaSemana = diaSemana;
            this.HoraInicio = horaInicio;
            this.HoraFin = horaFin;

        }

    }
}