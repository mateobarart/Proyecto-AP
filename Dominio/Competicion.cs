using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio
{
    [Table("Competiciones")]
    public class Competicion
    {
        [Key]
        public int IdCompeticion { get; set; }
        [Required]
        [DisplayName("Nombre")]
        public String NombreCompeticion { get; set; }
        [Required]
        public int Prioridad { get; set; } //va de 0 a 5
        [Required]
        public List<Equipo> Equipos { get; set; }
        [Required]
        public String Temporada { get; set; } //anio de la temporada, es string para los casos anioAnterior/anioActual 
    }
}