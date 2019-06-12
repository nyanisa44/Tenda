namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationTypeAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DocumentTypes", "ApplicationType_Id", c => c.Int());
            AddColumn("dbo.Applications", "ApplicationType_Id", c => c.Int());
            CreateIndex("dbo.DocumentTypes", "ApplicationType_Id");
            CreateIndex("dbo.Applications", "ApplicationType_Id");
            AddForeignKey("dbo.Applications", "ApplicationType_Id", "dbo.ApplicationTypes", "Id");
            AddForeignKey("dbo.DocumentTypes", "ApplicationType_Id", "dbo.ApplicationTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentTypes", "ApplicationType_Id", "dbo.ApplicationTypes");
            DropForeignKey("dbo.Applications", "ApplicationType_Id", "dbo.ApplicationTypes");
            DropIndex("dbo.Applications", new[] { "ApplicationType_Id" });
            DropIndex("dbo.DocumentTypes", new[] { "ApplicationType_Id" });
            DropColumn("dbo.Applications", "ApplicationType_Id");
            DropColumn("dbo.DocumentTypes", "ApplicationType_Id");
            DropTable("dbo.ApplicationTypes");
        }
    }
}
