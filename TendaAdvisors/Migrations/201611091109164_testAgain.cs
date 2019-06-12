namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testAgain : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Advisor2LicenceType", "AdvisorId", "dbo.Advisors");
            DropIndex("dbo.Advisor2LicenceType", new[] { "AdvisorId" });
            AddColumn("dbo.Advisor2LicenceType", "Advisor", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advisor2LicenceType", "Advisor");
            CreateIndex("dbo.Advisor2LicenceType", "AdvisorId");
            AddForeignKey("dbo.Advisor2LicenceType", "AdvisorId", "dbo.Advisors", "Id", cascadeDelete: true);
        }
    }
}
