namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkLicenseCategoriesToLicenseTypes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.LicenseTypes", "LicenseCategoryId");
            AddForeignKey("dbo.LicenseTypes", "LicenseCategoryId", "dbo.LicenseCategories", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LicenseTypes", "LicenseCategoryId", "dbo.LicenseCategories");
            DropIndex("dbo.LicenseTypes", new[] { "LicenseCategoryId" });
        }
    }
}
