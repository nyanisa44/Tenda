namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class undersuperviosn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advisor2LicenceType", "underSupervision", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advisor2LicenceType", "underSupervision");
        }
    }
}
