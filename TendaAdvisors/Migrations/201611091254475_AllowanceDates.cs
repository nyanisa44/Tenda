namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllowanceDates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advisor2LicenceType", "Allowance", c => c.Double(nullable: false));
            AddColumn("dbo.Advisor2LicenceType", "ValidFromDate", c => c.DateTime());
            AddColumn("dbo.Advisor2LicenceType", "ValidToDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advisor2LicenceType", "ValidToDate");
            DropColumn("dbo.Advisor2LicenceType", "ValidFromDate");
            DropColumn("dbo.Advisor2LicenceType", "Allowance");
        }
    }
}
