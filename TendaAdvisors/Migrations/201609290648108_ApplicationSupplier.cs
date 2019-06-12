namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationSupplier : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ApplicationProducts", newName: "ProductApplications");
            DropPrimaryKey("dbo.ProductApplications");
            CreateTable(
                "dbo.ApplicationSuppliers",
                c => new
                    {
                        ApplicationId = c.Int(nullable: false),
                        SupplierId = c.Int(nullable: false),
                        MemberNumber = c.String(),
                    })
                .PrimaryKey(t => new { t.ApplicationId, t.SupplierId })
                .ForeignKey("dbo.Applications", t => t.ApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId, cascadeDelete: true)
                .Index(t => t.ApplicationId)
                .Index(t => t.SupplierId);
            
            AddPrimaryKey("dbo.ProductApplications", new[] { "Product_Id", "Application_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationSuppliers", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.ApplicationSuppliers", "ApplicationId", "dbo.Applications");
            DropIndex("dbo.ApplicationSuppliers", new[] { "SupplierId" });
            DropIndex("dbo.ApplicationSuppliers", new[] { "ApplicationId" });
            DropPrimaryKey("dbo.ProductApplications");
            DropTable("dbo.ApplicationSuppliers");
            AddPrimaryKey("dbo.ProductApplications", new[] { "Application_Id", "Product_Id" });
            RenameTable(name: "dbo.ProductApplications", newName: "ApplicationProducts");
        }
    }
}
