namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComissionRunUpdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommissionStatements", "CommisionRunDate", c => c.DateTime());
            AddColumn("dbo.CommissionStatements", "CommisionRunUser", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CommissionStatements", "CommisionRunUser");
            DropColumn("dbo.CommissionStatements", "CommisionRunDate");
        }
    }
}
