using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Configuration;

namespace MVC.Models
{
    public class Context: DbContext 
    {
        public Context() : base("web") {
        }
        public DbSet<Equipo> DbEquipos { get; set; }
        public DbSet<Usuario> DbUsuarios { get; set; }
        public DbSet<Partido> DbPartidos { get; set; }
        public DbSet<Competicion> DbCompeticiones { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Partido>()
                .HasOptional<Usuario>(s => s.AnalistaLocal)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Partido>()
               .HasOptional<Equipo>(s => s.Local)
               .WithMany()
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Equipo>()
               .HasOptional<Usuario>(s => s.Titular)
               .WithMany()
               .WillCascadeOnDelete(false);
            modelBuilder.Entity<Equipo>()
               .HasOptional<Usuario>(s => s.Reserva)
               .WithMany()
               .WillCascadeOnDelete(false);
            modelBuilder.Entity<Equipo>()
               .HasOptional<Usuario>(s => s.Suplente)
               .WithMany()
               .WillCascadeOnDelete(false);
        }

        public System.Data.Entity.DbSet<MVC.Models.Indisponibilidad> Indisponibilidads { get; set; }
    }

    
}