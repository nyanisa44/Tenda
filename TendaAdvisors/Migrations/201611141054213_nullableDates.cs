namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullableDates : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Advisor2LicenceType", "validCommissionFromDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Advisor2LicenceType", "validCommissionToDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Advisor2LicenceType", "ValidFromDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Advisor2LicenceType", "ValidToDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Advisor2LicenceType", "ValidToDate", c => c.DateTime());
            AlterColumn("dbo.Advisor2LicenceType", "ValidFromDate", c => c.DateTime());
            AlterColumn("dbo.Advisor2LicenceType", "validCommissionToDate", c => c.DateTime());
            AlterColumn("dbo.Advisor2LicenceType", "validCommissionFromDate", c => c.DateTime());
        }
    }
}
