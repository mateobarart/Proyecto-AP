using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    [Table("Competiciones")]
    public class Competicion
    {
        [Key]
        public int IdCompeticion { get; set; }

        [Required]
        [DisplayName("Nombre")]
        public string NombreCompeticion { get; set; }

        [Range(0, 5)]
        public int Prioridad { get; set; } //va de 0 a 5

        public List<Equipo> Equipos { get; set; }

        public string Temporada { get; set; } //año de la temporada, es string para los casos añoAnterior/añoActual 

        public Competicion()
        {
            Equipos = new List<Equipo>();
        }

        public Competicion(string nombreCompeticion)
        {
            NombreCompeticion = nombreCompeticion;
        }

        public Competicion(string nombreCompeticion, string temporada, int prioridad) : this(nombreCompeticion)
        {
            NombreCompeticion = nombreCompeticion;
            Temporada = temporada;
            Prioridad = prioridad;
        }
    }
}