using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    [Table("IndisponibilidadUnica")]
    public class IndisponibilidadUnica:Indisponibilidad
    {
        public IndisponibilidadUnica() { }

      
        public IndisponibilidadUnica(DateTime fechaInicio, DateTime fechaFin, Usuario usuario)
        {
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
        }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [Required]
        [DisplayName("Fecha Inicio")]
        public DateTime FechaInicio { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [Required]
        [DisplayName("Fecha Fin")]
        public DateTime FechaFin { get; set; }

    }
}