namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdvisorSupplierLink : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AdvisorSupplierCodes", "SupplierId");
            CreateIndex("dbo.AdvisorSupplierCodes", "AdvisorId");
            AddForeignKey("dbo.AdvisorSupplierCodes", "AdvisorId", "dbo.Advisors", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AdvisorSupplierCodes", "SupplierId", "dbo.Suppliers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdvisorSupplierCodes", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.AdvisorSupplierCodes", "AdvisorId", "dbo.Advisors");
            DropIndex("dbo.AdvisorSupplierCodes", new[] { "AdvisorId" });
            DropIndex("dbo.AdvisorSupplierCodes", new[] { "SupplierId" });
        }
    }
}
