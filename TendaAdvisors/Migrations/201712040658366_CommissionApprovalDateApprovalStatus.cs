namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommissionApprovalDateApprovalStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommissionStatements", "ApprovalDate", c => c.DateTime(nullable:true));
            AddColumn("dbo.CommissionStatements", "ApprovalStatus", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CommissionStatements", "ApprovalStatus");
            DropColumn("dbo.CommissionStatements", "ApprovalDate");
        }
    }
}
