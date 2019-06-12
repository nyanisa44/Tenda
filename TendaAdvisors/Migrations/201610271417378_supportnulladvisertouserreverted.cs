namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class supportnulladvisertouserreverted : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "AdvisorId", "dbo.Advisors");
            DropIndex("dbo.AspNetUsers", new[] { "AdvisorId" });
            AlterColumn("dbo.Advisors", "User_AdvisorId", c => c.Int(nullable: false));
            AlterColumn("dbo.AspNetUsers", "AdvisorId", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "AdvisorId");
            AddForeignKey("dbo.AspNetUsers", "AdvisorId", "dbo.Advisors", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "AdvisorId", "dbo.Advisors");
            DropIndex("dbo.AspNetUsers", new[] { "AdvisorId" });
            AlterColumn("dbo.AspNetUsers", "AdvisorId", c => c.Int());
            AlterColumn("dbo.Advisors", "User_AdvisorId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "AdvisorId");
            AddForeignKey("dbo.AspNetUsers", "AdvisorId", "dbo.Advisors", "Id");
        }
    }
}
