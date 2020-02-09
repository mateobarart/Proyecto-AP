using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    [Table("Partidos")]
    public class Partido
    {
        public Partido()
        {
        }

        public Partido(DateTime fechaPartido, string hora, Competicion competicion, Equipo local, Equipo visitante)
        {
            FechaPartido = fechaPartido;
            Hora = hora;
            Competicion = competicion;
            Local = local;
            Visitante = visitante;
        }

        [Key]
        public int IdPartido { get; set; }
       
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [Required]
        [DisplayName("Fecha")]
        public DateTime FechaPartido { get; set; }
        
        [Required]
        [DisplayName("Hora")]
        public string Hora { get; set; }
       
        [Required]
        [DisplayName("Competición")]
        public virtual Competicion Competicion { get; set; }
        
        [Required]
        [DisplayName("Equipo local")]
        public virtual Equipo Local { get; set; }
        [Required]
        [DisplayName("Equipo visitante")]
        public virtual Equipo Visitante { get; set; }
       
        [DisplayName("Analista local")]
        public virtual Usuario AnalistaLocal { get; set; }
       
        [DisplayName("Analista visitante")]
        public virtual Usuario AnalistaVisitante { get; set; }
       


    }
}