using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    [Table("Indisponibilidades")]
    public abstract class Indisponibilidad
    {

        public Indisponibilidad(){ }

        protected Indisponibilidad(Usuario usuario)
        {
            Usuario = usuario;
        }

        [Key]
        public int IdIndisponibilidad { get; set; }

        

        public Usuario Usuario { get; set; }



    }
}