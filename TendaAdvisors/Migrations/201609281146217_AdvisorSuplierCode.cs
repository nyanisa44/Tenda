namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdvisorSuplierCode : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Queries", "Advisor_Id", "dbo.Advisors");
            AddColumn("dbo.Queries", "Advisor_Id1", c => c.Int());
            AddColumn("dbo.Queries", "Advisor_Id2", c => c.Int());
            AlterColumn("dbo.AdvisorSupplierCodes", "AdvisorCode", c => c.String());
            CreateIndex("dbo.Queries", "Advisor_Id1");
            CreateIndex("dbo.Queries", "Advisor_Id2");
            AddForeignKey("dbo.Queries", "Advisor_Id2", "dbo.Advisors", "Id");
            AddForeignKey("dbo.Queries", "Advisor_Id1", "dbo.Advisors", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Queries", "Advisor_Id1", "dbo.Advisors");
            DropForeignKey("dbo.Queries", "Advisor_Id2", "dbo.Advisors");
            DropIndex("dbo.Queries", new[] { "Advisor_Id2" });
            DropIndex("dbo.Queries", new[] { "Advisor_Id1" });
            AlterColumn("dbo.AdvisorSupplierCodes", "AdvisorCode", c => c.Int(nullable: false));
            DropColumn("dbo.Queries", "Advisor_Id2");
            DropColumn("dbo.Queries", "Advisor_Id1");
            AddForeignKey("dbo.Queries", "Advisor_Id", "dbo.Advisors", "Id");
        }
    }
}
