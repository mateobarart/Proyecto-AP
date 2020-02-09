using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio
{
    [Table("Partidos")]
    public class Partido
    {
        [Key]
        public int IdPartido { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [Required]
        [DisplayName("Fecha y hora")]
        public DateTime FechaHoraPartido { get; set; }
       /* [Required]
        [DisplayName("Equipo local")]
        public Equipo Local { get; set; }
        [Required]
        [DisplayName("Equipo visitante")]
        public Equipo Visitante { get; set; }*/
        [Required]
        [DisplayName("Analista local")]
        public Usuario AnalistaLocal { get; set; }
        [Required]
        [DisplayName("Analista visitante")]
        public Usuario AnalistaVisitante { get; set; }
       /*[Required]
        public List<Usuario> Analistas { get; set; }*/
        [Required]
        public List<Equipo> Equipos { get; set; }
        [Required]
        [DisplayName("Competición")]
        public Competicion Competicion { get; set; }


    }
}