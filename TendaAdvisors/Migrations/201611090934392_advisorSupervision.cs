namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class advisorSupervision : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Advisor2LicenceType", "AdvisorId");
            AddForeignKey("dbo.Advisor2LicenceType", "AdvisorId", "dbo.Advisors", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Advisor2LicenceType", "AdvisorId", "dbo.Advisors");
            DropIndex("dbo.Advisor2LicenceType", new[] { "AdvisorId" });
        }
    }
}
