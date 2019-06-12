namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationAdvisorHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationAdvisorHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateStarted = c.DateTime(nullable: false),
                        DateEnded = c.DateTime(),
                        Application_Id = c.Int(nullable: false),
                        Old_Advisor = c.Int(nullable: false),
                        New_Advisor = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Advisors", t => t.Old_Advisor, cascadeDelete: false)
                .ForeignKey("dbo.Applications", t => t.Application_Id, cascadeDelete: false)
                .ForeignKey("dbo.Advisors", t => t.New_Advisor, cascadeDelete: false)
                .Index(t => t.Application_Id)
                .Index(t => t.Old_Advisor)
                .Index(t => t.New_Advisor);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationAdvisorHistories", "New_Advisor", "dbo.Advisors");
            DropForeignKey("dbo.ApplicationAdvisorHistories", "Application_Id", "dbo.Applications");
            DropForeignKey("dbo.ApplicationAdvisorHistories", "Old_Advisor", "dbo.Advisors");
            DropIndex("dbo.ApplicationAdvisorHistories", new[] { "New_Advisor" });
            DropIndex("dbo.ApplicationAdvisorHistories", new[] { "Old_Advisor" });
            DropIndex("dbo.ApplicationAdvisorHistories", new[] { "Application_Id" });
            DropTable("dbo.ApplicationAdvisorHistories");
        }
    }
}
