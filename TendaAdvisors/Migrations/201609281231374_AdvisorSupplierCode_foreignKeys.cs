namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdvisorSupplierCode_foreignKeys : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ProductApplications", newName: "ApplicationProducts");
            RenameTable(name: "dbo.SupplierLicenseTypes", newName: "LicenseTypeSuppliers");
            DropPrimaryKey("dbo.ApplicationProducts");
            DropPrimaryKey("dbo.LicenseTypeSuppliers");
            AddPrimaryKey("dbo.ApplicationProducts", new[] { "Application_Id", "Product_Id" });
            AddPrimaryKey("dbo.LicenseTypeSuppliers", new[] { "LicenseType_Id", "Supplier_Id" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.LicenseTypeSuppliers");
            DropPrimaryKey("dbo.ApplicationProducts");
            AddPrimaryKey("dbo.LicenseTypeSuppliers", new[] { "Supplier_Id", "LicenseType_Id" });
            AddPrimaryKey("dbo.ApplicationProducts", new[] { "Product_Id", "Application_Id" });
            RenameTable(name: "dbo.LicenseTypeSuppliers", newName: "SupplierLicenseTypes");
            RenameTable(name: "dbo.ApplicationProducts", newName: "ProductApplications");
        }
    }
}
