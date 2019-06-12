namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommissionTax : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommissionStatements", "AdvisorTax", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.CommissionStatements", "AdvisorTaxRate", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CommissionStatements", "AdvisorTaxRate");
            DropColumn("dbo.CommissionStatements", "AdvisorTax");
        }
    }
}
