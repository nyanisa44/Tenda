namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateToCommision : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advisor2LicenceType", "validCommissionFromDate", c => c.DateTime());
            AddColumn("dbo.Advisor2LicenceType", "validCommissionToDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advisor2LicenceType", "validCommissionToDate");
            DropColumn("dbo.Advisor2LicenceType", "validCommissionFromDate");
        }
    }
}
