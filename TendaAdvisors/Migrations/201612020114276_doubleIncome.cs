namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class doubleIncome : DbMigration
    {
        public override void Up()
        {
           // AlterColumn("dbo.Advisors", "Allowance", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
         //   AlterColumn("dbo.Advisors", "Allowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
