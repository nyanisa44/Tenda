namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationAdvisorEdiHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationAdvisorEditHistories",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DateEdited = c.DateTime(nullable: false),
                    ApplicationId = c.Int(nullable: false),
                    AdvisorId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ApplicationAdvisorEditHistories");
        }
    }
}
