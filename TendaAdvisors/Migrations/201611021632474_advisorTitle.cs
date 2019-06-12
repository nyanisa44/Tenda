namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class advisorTitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advisors", "ContactTitle_Id", c => c.Int());
            CreateIndex("dbo.Advisors", "ContactTitle_Id");
            AddForeignKey("dbo.Advisors", "ContactTitle_Id", "dbo.ContactTitles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Advisors", "ContactTitle_Id", "dbo.ContactTitles");
            DropIndex("dbo.Advisors", new[] { "ContactTitle_Id" });
            DropColumn("dbo.Advisors", "ContactTitle_Id");
        }
    }
}
