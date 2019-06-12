namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removAllowanceFromLicenses : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Advisor2LicenceType", "Allowance");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Advisor2LicenceType", "Allowance", c => c.Double(nullable: false));
        }
    }
}
