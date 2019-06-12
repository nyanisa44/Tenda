namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advisors", "DateAuthorized", c => c.DateTime());
            AddColumn("dbo.Advisors", "ComplianceOfficer", c => c.String());
            AddColumn("dbo.Advisors", "ComplianceOffficerNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advisors", "ComplianceOffficerNumber");
            DropColumn("dbo.Advisors", "ComplianceOfficer");
            DropColumn("dbo.Advisors", "DateAuthorized");
        }
    }
}
