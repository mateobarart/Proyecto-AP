namespace MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class startWeb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Competiciones",
                c => new
                    {
                        IdCompeticion = c.Int(nullable: false, identity: true),
                        NombreCompeticion = c.String(nullable: false),
                        Prioridad = c.Int(nullable: false),
                        Temporada = c.String(),
                    })
                .PrimaryKey(t => t.IdCompeticion);
            
            CreateTable(
                "dbo.Equipos",
                c => new
                    {
                        IdEquipo = c.Int(nullable: false, identity: true),
                        NombreEquipo = c.String(nullable: false),
                        Reserva_IdUsuario = c.Int(),
                        Suplente_IdUsuario = c.Int(),
                        Titular_IdUsuario = c.Int(),
                        Competicion_IdCompeticion = c.Int(),
                    })
                .PrimaryKey(t => t.IdEquipo)
                .ForeignKey("dbo.Usuarios", t => t.Reserva_IdUsuario)
                .ForeignKey("dbo.Usuarios", t => t.Suplente_IdUsuario)
                .ForeignKey("dbo.Usuarios", t => t.Titular_IdUsuario)
                .ForeignKey("dbo.Competiciones", t => t.Competicion_IdCompeticion)
                .Index(t => t.Reserva_IdUsuario)
                .Index(t => t.Suplente_IdUsuario)
                .Index(t => t.Titular_IdUsuario)
                .Index(t => t.Competicion_IdCompeticion);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        IdUsuario = c.Int(nullable: false, identity: true),
                        NombreUsuario = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        TipoUsuario = c.Int(nullable: false),
                        Mail = c.String(nullable: false),
                        Puntaje = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Indisponibilidades",
                c => new
                    {
                        IdIndisponibilidad = c.Int(nullable: false, identity: true),
                        Usuario_IdUsuario = c.Int(),
                    })
                .PrimaryKey(t => t.IdIndisponibilidad)
                .ForeignKey("dbo.Usuarios", t => t.Usuario_IdUsuario)
                .Index(t => t.Usuario_IdUsuario);
            
            CreateTable(
                "dbo.Partidos",
                c => new
                    {
                        IdPartido = c.Int(nullable: false, identity: true),
                        FechaPartido = c.DateTime(nullable: false),
                        Hora = c.String(nullable: false),
                        AnalistaLocal_IdUsuario = c.Int(),
                        AnalistaVisitante_IdUsuario = c.Int(),
                        Competicion_IdCompeticion = c.Int(nullable: false),
                        Local_IdEquipo = c.Int(),
                        Visitante_IdEquipo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdPartido)
                .ForeignKey("dbo.Usuarios", t => t.AnalistaLocal_IdUsuario)
                .ForeignKey("dbo.Usuarios", t => t.AnalistaVisitante_IdUsuario)
                .ForeignKey("dbo.Competiciones", t => t.Competicion_IdCompeticion, cascadeDelete: true)
                .ForeignKey("dbo.Equipos", t => t.Local_IdEquipo)
                .ForeignKey("dbo.Equipos", t => t.Visitante_IdEquipo, cascadeDelete: true)
                .Index(t => t.AnalistaLocal_IdUsuario)
                .Index(t => t.AnalistaVisitante_IdUsuario)
                .Index(t => t.Competicion_IdCompeticion)
                .Index(t => t.Local_IdEquipo)
                .Index(t => t.Visitante_IdEquipo);
            
            CreateTable(
                "dbo.IndisponibilidadRecurrente",
                c => new
                    {
                        IdIndisponibilidad = c.Int(nullable: false),
                        DiaSemana = c.String(nullable: false),
                        HoraInicio = c.String(nullable: false),
                        HoraFin = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IdIndisponibilidad)
                .ForeignKey("dbo.Indisponibilidades", t => t.IdIndisponibilidad)
                .Index(t => t.IdIndisponibilidad);
            
            CreateTable(
                "dbo.IndisponibilidadUnica",
                c => new
                    {
                        IdIndisponibilidad = c.Int(nullable: false),
                        FechaInicio = c.DateTime(nullable: false),
                        FechaFin = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IdIndisponibilidad)
                .ForeignKey("dbo.Indisponibilidades", t => t.IdIndisponibilidad)
                .Index(t => t.IdIndisponibilidad);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IndisponibilidadUnica", "IdIndisponibilidad", "dbo.Indisponibilidades");
            DropForeignKey("dbo.IndisponibilidadRecurrente", "IdIndisponibilidad", "dbo.Indisponibilidades");
            DropForeignKey("dbo.Partidos", "Visitante_IdEquipo", "dbo.Equipos");
            DropForeignKey("dbo.Partidos", "Local_IdEquipo", "dbo.Equipos");
            DropForeignKey("dbo.Partidos", "Competicion_IdCompeticion", "dbo.Competiciones");
            DropForeignKey("dbo.Partidos", "AnalistaVisitante_IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.Partidos", "AnalistaLocal_IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.Equipos", "Competicion_IdCompeticion", "dbo.Competiciones");
            DropForeignKey("dbo.Equipos", "Titular_IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.Equipos", "Suplente_IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.Equipos", "Reserva_IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.Indisponibilidades", "Usuario_IdUsuario", "dbo.Usuarios");
            DropIndex("dbo.IndisponibilidadUnica", new[] { "IdIndisponibilidad" });
            DropIndex("dbo.IndisponibilidadRecurrente", new[] { "IdIndisponibilidad" });
            DropIndex("dbo.Partidos", new[] { "Visitante_IdEquipo" });
            DropIndex("dbo.Partidos", new[] { "Local_IdEquipo" });
            DropIndex("dbo.Partidos", new[] { "Competicion_IdCompeticion" });
            DropIndex("dbo.Partidos", new[] { "AnalistaVisitante_IdUsuario" });
            DropIndex("dbo.Partidos", new[] { "AnalistaLocal_IdUsuario" });
            DropIndex("dbo.Indisponibilidades", new[] { "Usuario_IdUsuario" });
            DropIndex("dbo.Equipos", new[] { "Competicion_IdCompeticion" });
            DropIndex("dbo.Equipos", new[] { "Titular_IdUsuario" });
            DropIndex("dbo.Equipos", new[] { "Suplente_IdUsuario" });
            DropIndex("dbo.Equipos", new[] { "Reserva_IdUsuario" });
            DropTable("dbo.IndisponibilidadUnica");
            DropTable("dbo.IndisponibilidadRecurrente");
            DropTable("dbo.Partidos");
            DropTable("dbo.Indisponibilidades");
            DropTable("dbo.Usuarios");
            DropTable("dbo.Equipos");
            DropTable("dbo.Competiciones");
        }
    }
}
