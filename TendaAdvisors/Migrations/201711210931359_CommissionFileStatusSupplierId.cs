namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommissionFileStatusSupplierId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommissionFileStatus", "SupplierId", c => c.Int(nullable: true));
            CreateIndex("dbo.CommissionFileStatus", "SupplierId");
            AddForeignKey("dbo.CommissionFileStatus", "SupplierId", "dbo.Suppliers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommissionFileStatus", "SupplierId", "dbo.Suppliers");
            DropIndex("dbo.CommissionFileStatus", new[] { "SupplierId" });
            DropColumn("dbo.CommissionFileStatus", "SupplierId");
        }
    }
}
