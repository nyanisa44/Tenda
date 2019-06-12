namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReverseAdvisorSupervision : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Advisor2LicenceType", "AdvisorId", "dbo.Advisors");
            DropIndex("dbo.Advisor2LicenceType", new[] { "AdvisorId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Advisor2LicenceType", "AdvisorId");
            AddForeignKey("dbo.Advisor2LicenceType", "AdvisorId", "dbo.Advisors", "Id", cascadeDelete: true);
        }
    }
}
