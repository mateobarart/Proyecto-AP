namespace MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usuariosActivos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usuarios", "Activo", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Usuarios", "Activo");
        }
    }
}
