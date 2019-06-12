namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommisionStatement1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommissionStatements", "AdvisorShare", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.CommissionStatements", "CompanyShare", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.CommissionStatements", "Share");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CommissionStatements", "Share", c => c.Int());
            DropColumn("dbo.CommissionStatements", "CompanyShare");
            DropColumn("dbo.CommissionStatements", "AdvisorShare");
        }
    }
}
