namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentsToApplicationTypes : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.LicenseTypeSuppliers", newName: "SupplierLicenseTypes");
            DropForeignKey("dbo.DocumentTypes", "ApplicationType_Id", "dbo.ApplicationTypes");
            DropIndex("dbo.DocumentTypes", new[] { "ApplicationType_Id" });
            DropPrimaryKey("dbo.SupplierLicenseTypes");
            CreateTable(
                "dbo.ApplicationTypeDocumentTypes",
                c => new
                    {
                        ApplicationType_Id = c.Int(nullable: false),
                        DocumentType_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationType_Id, t.DocumentType_Id })
                .ForeignKey("dbo.ApplicationTypes", t => t.ApplicationType_Id, cascadeDelete: true)
                .ForeignKey("dbo.DocumentTypes", t => t.DocumentType_Id, cascadeDelete: true)
                .Index(t => t.ApplicationType_Id)
                .Index(t => t.DocumentType_Id);
            
            AddPrimaryKey("dbo.SupplierLicenseTypes", new[] { "Supplier_Id", "LicenseType_Id" });
            DropColumn("dbo.DocumentTypes", "ApplicationType_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DocumentTypes", "ApplicationType_Id", c => c.Int());
            DropForeignKey("dbo.ApplicationTypeDocumentTypes", "DocumentType_Id", "dbo.DocumentTypes");
            DropForeignKey("dbo.ApplicationTypeDocumentTypes", "ApplicationType_Id", "dbo.ApplicationTypes");
            DropIndex("dbo.ApplicationTypeDocumentTypes", new[] { "DocumentType_Id" });
            DropIndex("dbo.ApplicationTypeDocumentTypes", new[] { "ApplicationType_Id" });
            DropPrimaryKey("dbo.SupplierLicenseTypes");
            DropTable("dbo.ApplicationTypeDocumentTypes");
            AddPrimaryKey("dbo.SupplierLicenseTypes", new[] { "LicenseType_Id", "Supplier_Id" });
            CreateIndex("dbo.DocumentTypes", "ApplicationType_Id");
            AddForeignKey("dbo.DocumentTypes", "ApplicationType_Id", "dbo.ApplicationTypes", "Id");
            RenameTable(name: "dbo.SupplierLicenseTypes", newName: "LicenseTypeSuppliers");
        }
    }
}
