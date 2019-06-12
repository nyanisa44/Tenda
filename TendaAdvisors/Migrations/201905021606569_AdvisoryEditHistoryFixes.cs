namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdvisoryEditHistoryFixes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationAdvisorEditHistories", "AdvisorId", "dbo.Advisors");
            DropForeignKey("dbo.ApplicationAdvisorEditHistories", "Id", "dbo.Applications");
            DropIndex("dbo.ApplicationAdvisorEditHistories", new[] { "Id" });
            DropIndex("dbo.ApplicationAdvisorEditHistories", new[] { "AdvisorId" });
            DropPrimaryKey("dbo.ApplicationAdvisorEditHistories");
            AlterColumn("dbo.ApplicationAdvisorEditHistories", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.ApplicationAdvisorEditHistories", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ApplicationAdvisorEditHistories");
            AlterColumn("dbo.ApplicationAdvisorEditHistories", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.ApplicationAdvisorEditHistories", "Id");
            CreateIndex("dbo.ApplicationAdvisorEditHistories", "AdvisorId");
            CreateIndex("dbo.ApplicationAdvisorEditHistories", "Id");
            AddForeignKey("dbo.ApplicationAdvisorEditHistories", "Id", "dbo.Applications", "Id");
            AddForeignKey("dbo.ApplicationAdvisorEditHistories", "AdvisorId", "dbo.Advisors", "Id", cascadeDelete: true);
        }
    }
}
