namespace MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class clavesUnicas : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Equipos", "NombreEquipo", c => c.String(nullable: false, maxLength: 450));
            AlterColumn("dbo.Usuarios", "NombreUsuario", c => c.String(nullable: false, maxLength: 450));
            AlterColumn("dbo.Usuarios", "Mail", c => c.String(nullable: false, maxLength: 450));
            CreateIndex("dbo.Equipos", "NombreEquipo", unique: true);
            CreateIndex("dbo.Usuarios", "NombreUsuario", unique: true);
            CreateIndex("dbo.Usuarios", "Mail", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Usuarios", new[] { "Mail" });
            DropIndex("dbo.Usuarios", new[] { "NombreUsuario" });
            DropIndex("dbo.Equipos", new[] { "NombreEquipo" });
            AlterColumn("dbo.Usuarios", "Mail", c => c.String(nullable: false));
            AlterColumn("dbo.Usuarios", "NombreUsuario", c => c.String(nullable: false));
            AlterColumn("dbo.Equipos", "NombreEquipo", c => c.String(nullable: false));
        }
    }
}
