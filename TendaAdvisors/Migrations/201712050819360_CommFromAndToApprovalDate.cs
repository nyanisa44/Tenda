namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommFromAndToApprovalDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommissionStatements", "ApprovalDateFrom", c => c.DateTime(nullable:true));
            AddColumn("dbo.CommissionStatements", "ApprovalDateTo", c => c.DateTime(nullable:true));
            DropColumn("dbo.CommissionStatements", "ApprovalDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CommissionStatements", "ApprovalDate", c => c.DateTime());
            DropColumn("dbo.CommissionStatements", "ApprovalDateTo");
            DropColumn("dbo.CommissionStatements", "ApprovalDateFrom");
        }
    }
}
