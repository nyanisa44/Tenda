namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnmatchedAndCommissionSupplierId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommissionStatements", "SupplierId", c => c.Int(nullable:true));
            AddColumn("dbo.UnmatchedCommissions", "SupplierId", c => c.Int(nullable: true));
            CreateIndex("dbo.CommissionStatements", "SupplierId");
            CreateIndex("dbo.UnmatchedCommissions", "SupplierId");
            AddForeignKey("dbo.CommissionStatements", "SupplierId", "dbo.Suppliers", "Id");
            AddForeignKey("dbo.UnmatchedCommissions", "SupplierId", "dbo.Suppliers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UnmatchedCommissions", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.CommissionStatements", "SupplierId", "dbo.Suppliers");
            DropIndex("dbo.UnmatchedCommissions", new[] { "SupplierId" });
            DropIndex("dbo.CommissionStatements", new[] { "SupplierId" });
            DropColumn("dbo.UnmatchedCommissions", "SupplierId");
            DropColumn("dbo.CommissionStatements", "SupplierId");
        }
    }
}
