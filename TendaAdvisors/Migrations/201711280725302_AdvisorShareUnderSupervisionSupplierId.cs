namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdvisorShareUnderSupervisionSupplierId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdvisorShareUnderSupervisions", "SupplierId", c => c.Int(nullable:true));
            CreateIndex("dbo.AdvisorShareUnderSupervisions", "SupplierId");
            AddForeignKey("dbo.AdvisorShareUnderSupervisions", "SupplierId", "dbo.Suppliers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdvisorShareUnderSupervisions", "SupplierId", "dbo.Suppliers");
            DropIndex("dbo.AdvisorShareUnderSupervisions", new[] { "SupplierId" });
            DropColumn("dbo.AdvisorShareUnderSupervisions", "SupplierId");
        }
    }
}
